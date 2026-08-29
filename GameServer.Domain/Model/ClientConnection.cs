using GameServer.Application.Interfaces;
using GameServer.Application.Messaging;
using GameServer.Application.Sessions;
using GameServer.Contracts.MessageFormats;
using GameServer.Services.WS.WSMiddleware;
using System.Buffers;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Channels;

namespace GameServer.Application.Model
{
    public class ClientConnection : IAsyncDisposable, IDisposable
    {
        private readonly IConnectedClientsStorage _connectedClientsStorage;
        private readonly IWsContextFactory _wsContextFactory;
        private readonly MessagePipeline _pipeline;


        public string UserId { get; private set; }
        public uint UdpToken { get; private set; }



        public EndPoint ConnectionEndPoint { get; set; }

        private WebSocket _socket;
        private WsContext _context;

        public GameSession Session { get; private set; }
        public IWsRouter Router { get; private set; }



        private readonly Channel<OutgoingMessage> _sendChannel = Channel.CreateUnbounded<OutgoingMessage>();

        private Task _sendingLoop;
        private Task _receivingLoop;

        public Task Completion =>
            Task.WhenAll(_receivingLoop, _sendingLoop);


        private readonly SemaphoreSlim _readSemaphore =
    new SemaphoreSlim(0, 1);

        private readonly SemaphoreSlim _writeSemaphore =
            new SemaphoreSlim(0, 1);

        private volatile bool _isPausedRead = true;
        private volatile bool _isPausedWrite = true;


        private CancellationTokenSource _cancelAllToken;
        private TaskCompletionSource<bool> _clientReadyTcs;

        private int _isDisposed = 0;


        public async Task<bool> ClientReady(CancellationToken token)
        {
            try
            {
                return await _clientReadyTcs.Task.WaitAsync(token);
            }
            catch (OperationCanceledException)
            {
                return false;
            }
        }

        public ClientConnection(IWsContextFactory wsFactory, MessagePipeline messagePipeline, IConnectedClientsStorage clientStorage)
        {
            _wsContextFactory = wsFactory;
            _pipeline = messagePipeline;
            _connectedClientsStorage = clientStorage;
        }

        public void Init(string UserId, uint UdpToken, GameSession session, WebSocket socket, CancellationToken kestrelToken)
        {
            _clientReadyTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            this.UserId = UserId;
            this.UdpToken = UdpToken;
            this._socket = socket;
            this.Session = session;

            _context = _wsContextFactory.Create(_socket, UserId, Session.Id);
            Router = _context.Router;

            _cancelAllToken = CancellationTokenSource.CreateLinkedTokenSource(kestrelToken);
            SybscribeToclientReady();

            _sendingLoop = WriteLoopAsync(_cancelAllToken.Token);
            _receivingLoop = ReadLoopAsync(_cancelAllToken.Token);
        }

        private void SybscribeToclientReady()
        {
            Router.On("ClientReady", SetClienReady);
        }

        private async Task SetClienReady(WsMessageContext context)
        {
            _clientReadyTcs.TrySetResult(true);

            Router.Off("ClientReady", SetClienReady);
        }

        public Task WaitClientActionAsync(string action, CancellationToken ct)
        {
            return Router.WaitBeforeAsync(action, ct);
        }


        public void StartReadingLoop()
        {
            if (!_isPausedRead) return;
            _isPausedRead = false;
            _readSemaphore.Release();
            Console.WriteLine("Поток чтения запущен.");
        }

        public void PauseReadingLoop()
        {
            if (_isPausedRead) return;
            _isPausedRead = true;
            Console.WriteLine("Поток чтения на паузе.");
        }

        public void StartWritingLoop()
        {
            if (!_isPausedWrite) return;
            _isPausedWrite = false;
            _writeSemaphore.Release();
            Console.WriteLine("Поток записи запущен.");
        }

        public void PauseWritingLoop()
        {
            if (_isPausedWrite) return;
            _isPausedWrite = true;
            Console.WriteLine("Поток записи на паузе.");
        }

        public void SendPooled(byte[] data, int length, MessageFormat messageFormat)
        {
            if (_isDisposed == 1) return;

            _sendChannel.Writer.TryWrite(new OutgoingMessage(data, length, returnToPool: true, messageFormat: messageFormat));
        }

        public void SendShared(byte[] data, int length, MessageFormat messageFormat)
        {
            if (_isDisposed == 1) return;

            _sendChannel.Writer.TryWrite(new OutgoingMessage(data, length, returnToPool: false, messageFormat: messageFormat));
        }

        private async Task ReadLoopAsync(CancellationToken ct)
        {
            byte[] receiveBuffer = ArrayPool<byte>.Shared.Rent(8 * 1024);

            using var messageStream = new MemoryStream();

            try
            {
                while (!ct.IsCancellationRequested &&
                    _socket.State == WebSocketState.Open)
                {
                    if (_isPausedRead)
                    {
                        await _readSemaphore.WaitAsync(ct);
                    }

                    messageStream.SetLength(0);

                    WebSocketReceiveResult result;

                    do
                    {
                        result = await _socket.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), ct);

                        if (result.Count > 0)
                        {
                            messageStream.Write(receiveBuffer, 0, result.Count);
                        }
                    }
                    while (!result.EndOfMessage && _socket.State == WebSocketState.Open);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await this.CloseAsync();
                        break;
                    }


                    var messageContext = new WsMessageContext
                    {
                        ConnectionContext = _context
                    };

                    if (messageStream.TryGetBuffer(out ArraySegment<byte> streamBuffer))
                    {
                        var actualDataSpan = streamBuffer.AsSpan(0, (int)messageStream.Length);

                        if (result.MessageType == WebSocketMessageType.Text)
                        {
                            messageContext.Text = Encoding.UTF8.GetString(actualDataSpan);
                        }
                        else
                        {
                            messageContext.Binary = streamBuffer.Slice(0, (int)messageStream.Length);
                        }

                        messageContext = await _pipeline.Execute(messageContext);
                        await Router.HandleAsync(messageContext);
                    }
                }
            }
            catch (WebSocketException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (OperationCanceledException)
            {

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {

                ArrayPool<byte>.Shared.Return(receiveBuffer);

                if (Session != null)
                {
                    Session.RemovePlayer(UserId);
                }

                await _connectedClientsStorage.Remove(UserId);
                if (_socket.State != WebSocketState.Closed || _socket.State != WebSocketState.Aborted)
                {
                    Abort();
                }
            }
        }

        private async Task WriteLoopAsync(CancellationToken ct)
        {
            try
            {
                await foreach (var message in _sendChannel.Reader.ReadAllAsync(ct))
                {
                    try
                    {
                        if (_isPausedWrite)
                        {
                            await _writeSemaphore.WaitAsync(ct);
                        }

                        ct.ThrowIfCancellationRequested();

                        if (_socket.State != WebSocketState.Open)
                            break;

                        var segment = new ArraySegment<byte>(message.Buffer, 0, message.Length);

                        switch (message.MessageFormat)
                        {
                            case Contracts.MessageFormats.MessageFormat.Json:
                                {
                                    await _socket.SendAsync(
                                            segment,
                                            WebSocketMessageType.Text,
                                            endOfMessage: true,
                                            CancellationToken.None);
                                }
                                break;
                            case Contracts.MessageFormats.MessageFormat.Binary:
                            default:
                                {
                                    await _socket.SendAsync(
                                            segment,
                                            WebSocketMessageType.Binary,
                                            endOfMessage: true,
                                            CancellationToken.None);
                                }
                                break;
                        }


                    }catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    finally
                    {
                        if (message.ReturnToPool)
                        {
                            ArrayPool<byte>.Shared.Return(message.Buffer);
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка отправки: {ex.Message}");
            }
            finally
            {
                Abort();
            }
        }

        public async Task CloseAsync()
        {
            if (Interlocked.Exchange(ref _isDisposed, 1) == 1) return;

            _sendChannel.Writer.Complete();
            _cancelAllToken.Cancel();

            if (_socket.State == WebSocketState.Open || _socket.State == WebSocketState.CloseReceived)
            {
                try
                {
                    await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by server", CancellationToken.None);
                }
                catch
                {

                }
            }
        }

        public void Abort()
        {
            if (Interlocked.Exchange(ref _isDisposed, 1) == 1) return;

            _cancelAllToken.Cancel();
            _sendChannel.Writer.TryComplete();
            _socket.Abort();
        }

        public async ValueTask DisposeAsync()
        {
            await CloseAsync();
        }

        public void Dispose()
        {
            Router.Off("ClientReady", SetClienReady);

            Abort();
        }
    }
}
