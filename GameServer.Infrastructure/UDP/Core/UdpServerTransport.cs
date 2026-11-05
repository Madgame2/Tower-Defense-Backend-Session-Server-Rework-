using GameServer.Application.Interfaces;
using GameServer.Application.Model;
using GameServer.Contracts.UDP.Enums;
using GameServer.Domain.UDP.Interfaces;
using GameServer.Domain.UDP.Meta;
using GameServer.Infrastructure.UDP.Core.Storages;
using GameServer.Infrastructure.UDP.Deserializer;
using GameServer.Infrastructure.UDP.Meta;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

namespace GameServer.Infrastructure.UDP.Core
{
    public class UdpServerTransport : BackgroundService, IUdpSender
    {
        private readonly ILogger<UdpServerTransport> _logger;
        private readonly IConfiguration _configuration;
        private readonly IConnectedClientsStorage _clientsRepository;

        private readonly PendingPacketStorage _pendingPacketStorage = new(4096);

        private int _sequenceId;

        private const int HeaderSize = 5;
        private const int ReceiveBufferSize = 2048;

        private Socket _socket;

        public UdpServerTransport(
            ILogger<UdpServerTransport> logger,
            IConfiguration configuration,
            IConnectedClientsStorage clientsRepository)
        {
            _logger = logger;
            _configuration = configuration;
            _clientsRepository = clientsRepository;
        }

        public async ValueTask SendAsync(
            EndPoint endPoint,
            ReadOnlyMemory<byte> payload,
            CancellationToken cancellationToken = default)
        {
            if (endPoint == null)
                throw new ArgumentNullException(nameof(endPoint));

            cancellationToken.ThrowIfCancellationRequested();

            if (_socket == null)
                throw new InvalidOperationException("UDP socket is not initialized.");

            int length = HeaderSize + payload.Length;

            byte[] buffer = ArrayPool<byte>.Shared.Rent(length);

            try
            {
                Span<byte> span = buffer.AsSpan(0, length);

                span[0] = (byte)TransportFlags.None;

                BinaryPrimitives.WriteUInt32LittleEndian(
                    span.Slice(1, 4),
                    0);

                payload.Span.CopyTo(span.Slice(HeaderSize));

                await _socket.SendToAsync(
                    new ReadOnlyMemory<byte>(buffer, 0, length),
                    SocketFlags.None,
                    endPoint,
                    cancellationToken);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        public async ValueTask SendReliableAsync(
            EndPoint endPoint,
            ReadOnlyMemory<byte> payload,
            Action<PacketDeliveryResult> onCompleted,
            CancellationToken cancellationToken = default)
        {
            if (endPoint == null)
                throw new ArgumentNullException(nameof(endPoint));

            if (onCompleted == null)
                throw new ArgumentNullException(nameof(onCompleted));

            if (cancellationToken.IsCancellationRequested)
            {
                InvokeCallback(
                    onCompleted,
                    new PacketDeliveryResult(
                        0,
                        PacketDeliveryStatus.Cancelled));

                return;
            }

            if (_socket == null)
            {
                InvokeCallback(
                    onCompleted,
                    new PacketDeliveryResult(
                        0,
                        PacketDeliveryStatus.TransportDisposed));

                return;
            }

            uint sequenceId = unchecked(
                (uint)Interlocked.Increment(ref _sequenceId));

            int length = HeaderSize + payload.Length;

            byte[] buffer = ArrayPool<byte>.Shared.Rent(length);

            Span<byte> span = buffer.AsSpan(0, length);

            span[0] = (byte)TransportFlags.Reliable;

            BinaryPrimitives.WriteUInt32LittleEndian(
                span.Slice(1, 4),
                sequenceId);

            payload.Span.CopyTo(span.Slice(HeaderSize));

            if (!_pendingPacketStorage.TryAdd(
                    sequenceId,
                    buffer,
                    length,
                    endPoint,
                    onCompleted,
                    cancellationToken,
                    out var lease))
            {
                ArrayPool<byte>.Shared.Return(buffer);

                PacketDeliveryStatus status =
                    cancellationToken.IsCancellationRequested
                        ? PacketDeliveryStatus.Cancelled
                        : PacketDeliveryStatus.QueueFull;

                InvokeCallback(
                    onCompleted,
                    new PacketDeliveryResult(sequenceId, status));

                return;
            }

            await SendReliableAttemptAsync(lease);
        }

        private async ValueTask SendReliableAttemptAsync(
    PendingPacketStorage.PacketLease lease)
        {
            try
            {
                await _socket.SendToAsync(
                    new ReadOnlyMemory<byte>(
                        lease.Buffer,
                        0,
                        lease.Length),
                    SocketFlags.None,
                    lease.EndPoint);
            }
            catch (ObjectDisposedException ex)
            {
                FailPacket(
                    lease.SequenceId,
                    PacketDeliveryStatus.TransportDisposed,
                    ex);
            }
            catch (SocketException ex)
            {
                // Не считаем пакет доставленным.
                // Он останется в PendingPacketStorage
                // и будет повторно отправлен.
                _logger.LogWarning(
                    ex,
                    "UDP send failed. SequenceId: {SequenceId}",
                    lease.SequenceId);
            }
            catch (Exception ex)
            {
                FailPacket(
                    lease.SequenceId,
                    PacketDeliveryStatus.SendFailed,
                    ex);
            }
            finally
            {
                _pendingPacketStorage.CompleteSend(lease.SequenceId);
            }
        }

        private void FailPacket(
            uint sequenceId,
            PacketDeliveryStatus status,
            Exception exception)
        {
            if (!_pendingPacketStorage.Fail(
                    sequenceId,
                    status,
                    exception,
                    out var callback,
                    out var result))
                return;

            InvokeCallback(callback, result);
        }

        private void InvokeCallback(
            Action<PacketDeliveryResult> callback,
            PacketDeliveryResult result)
        {
            if (callback == null)
                return;

            try
            {
                callback(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "UDP delivery callback failed. SequenceId: {SequenceId}",
                    result.SequenceId);
            }
        }

        private async ValueTask DoSendAsync(
            byte[] buffer,
            int length,
            EndPoint endPoint)
        {
            try
            {
                await _socket.SendToAsync(
                    new ReadOnlyMemory<byte>(buffer, 0, length),
                    SocketFlags.None,
                    endPoint);
            }
            catch (ObjectDisposedException)
            {
            }
            catch (SocketException ex)
            {
                _logger.LogWarning(ex, "UDP send failed.");
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }


        private async ValueTask SendReliableAsync(
            PendingPacketStorage.PacketLease lease)
        {
            try
            {
                await _socket.SendToAsync(
                    new ReadOnlyMemory<byte>(
                        lease.Buffer,
                        0,
                        lease.Length),
                    SocketFlags.None,
                    lease.EndPoint);
            }
            catch (ObjectDisposedException)
            {
            }
            catch (SocketException ex)
            {
                _logger.LogWarning(
                    ex,
                    "Reliable UDP send failed. SequenceId: {SequenceId}",
                    lease.SequenceId);
            }
            finally
            {
                _pendingPacketStorage.CompleteSend(lease.SequenceId);
            }
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            int port = _configuration.GetValue<int>("Server:Port");
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _socket.Bind(new IPEndPoint(IPAddress.Any, port));

            _logger.LogInformation($"UDP-сервер запущен на порту {port}...");

            return base.StartAsync(cancellationToken);
        }

        private async Task RetransmissionLoop(
            CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                while (_pendingPacketStorage.TryAcquireDue(
                           out var lease,
                           out var callback,
                           out var result))
                {
                    // Если lease существует — это повторная отправка.
                    if (lease.Buffer != null)
                    {
                        await SendReliableAttemptAsync(lease);
                    }
                    else
                    {
                        // Пакет отменён или исчерпал попытки.
                        InvokeCallback(callback, result);
                    }
                }

                await Task.Delay(25, cancellationToken);
            }
        }

        private async ValueTask SendAcknowledgementAsync(
    EndPoint endPoint,
    uint sequenceId)
        {
            byte[] buffer = ArrayPool<byte>.Shared.Rent(HeaderSize);

            try
            {
                Span<byte> span = buffer.AsSpan(0, HeaderSize);

                span[0] = (byte)TransportFlags.Acknowledgement;

                BinaryPrimitives.WriteUInt32LittleEndian(
                    span.Slice(1, 4),
                    sequenceId);

                await _socket.SendToAsync(
                    new ReadOnlyMemory<byte>(buffer, 0, HeaderSize),
                    SocketFlags.None,
                    endPoint);
            }
            catch (ObjectDisposedException)
            {
            }
            catch (SocketException ex)
            {
                _logger.LogWarning(ex, "UDP acknowledgement send failed.");
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        protected override async Task ExecuteAsync(
    CancellationToken stoppingToken)
        {
            byte[] buffer = ArrayPool<byte>.Shared.Rent(ReceiveBufferSize);

            EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

            Task retransmissionTask = RetransmissionLoop(stoppingToken);

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var result = await _socket.ReceiveFromAsync(
                        buffer,
                        SocketFlags.None,
                        remoteEndPoint,
                        stoppingToken);

                    EndPoint actualSenderEndPoint = result.RemoteEndPoint;

                    ReadOnlySpan<byte> receivedData =
                        buffer.AsSpan(0, result.ReceivedBytes);

                    ProcessPacket(receivedData, actualSenderEndPoint);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("UDP server is stopping.");
            }
            catch (SocketException ex) when (
                ex.SocketErrorCode == SocketError.OperationAborted ||
                ex.ErrorCode == 995)
            {
                _logger.LogInformation("UDP server stopped.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UDP server error.");
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);

                try
                {
                    await retransmissionTask;
                }
                catch (OperationCanceledException)
                {
                }
            }
        }

        public override async Task StopAsync(
            CancellationToken cancellationToken)
        {
            try
            {
                _socket?.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error while disposing UDP socket.");
            }

            await base.StopAsync(cancellationToken);

            _pendingPacketStorage.Dispose(InvokeCallback);
        }

        private void ProcessPacket(
    ReadOnlySpan<byte> data,
    EndPoint senderEndPoint)
        {
            // UdpToken + Flags + SequenceId
            if (data.Length < 9)
                return;

            uint udpToken = BinaryPrimitives.ReadUInt32LittleEndian(
                data.Slice(0, 4));

            if (!_clientsRepository.tryGetByUdpToken(
                    udpToken,
                    out var clientConnection))
                return;

            byte flags = data[4];

            uint sequenceId = BinaryPrimitives.ReadUInt32LittleEndian(
                data.Slice(5, 4));

            TransportFlags transportFlags = (TransportFlags)flags;

            // ACK от клиента.
            if (transportFlags.HasFlag(TransportFlags.Acknowledgement))
            {
                if (_pendingPacketStorage.Acknowledge(
                        sequenceId,
                        senderEndPoint,
                        out var callback,
                        out var result))
                {
                    InvokeCallback(callback, result);
                }

                return;
            }

            bool reliable = transportFlags.HasFlag(TransportFlags.Reliable);

            // ACK на надёжный пакет клиента.
            if (reliable)
            {
                _ = SendAcknowledgementAsync(
                    senderEndPoint,
                    sequenceId);
            }

            // Для игрового сообщения необходим PacketType.
            if (data.Length < 10)
                return;

            clientConnection.ConnectionEndPoint = senderEndPoint;

            var room = clientConnection.Session.CurrentRoom;

            if (room == null)
                return;

            byte type = data[9];

            var payload = data.Slice(10);

            room.Router.Route(
                room,
                clientConnection.UserId,
                udpToken,
                type,
                payload);
        }
    }
}
