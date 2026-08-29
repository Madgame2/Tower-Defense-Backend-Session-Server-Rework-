using GameServer.Application.Interfaces;
using GameServer.Application.Model;
using GameServer.Contracts.UDP.Enums;
using GameServer.Domain.UDP.Interfaces;
using GameServer.Infrastructure.UDP.Deserializer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Buffers;
using System.Buffers.Binary;
using System.Net;
using System.Net.Sockets;

namespace GameServer.Infrastructure.UDP.Core
{
    public class UdpServerTransport : BackgroundService, IUdpSender
    {
        private readonly ILogger<UdpServerTransport> _logger;
        private readonly IConfiguration _configuration;
        private readonly IConnectedClientsStorage _clientsRepository;


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

        public ValueTask SendAsync(EndPoint endPoint, ReadOnlyMemory<byte> payload)
        {
            if (endPoint == null) return ValueTask.CompletedTask;

            int totalLength = payload.Length;

            byte[] buffer = ArrayPool<byte>.Shared.Rent(totalLength);

            payload.Span.CopyTo(buffer.AsSpan(0, totalLength));

            return DoSendAsync(buffer, totalLength, endPoint);
        }

        private async ValueTask DoSendAsync(byte[] buffer, int length, EndPoint endPoint)
        {
            try
            {
                await _socket.SendToAsync(new ReadOnlyMemory<byte>(buffer, 0, length), SocketFlags.None, endPoint);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
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

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            byte[] buffer = ArrayPool<byte>.Shared.Rent(2048);
            EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var result = await _socket.ReceiveFromAsync(buffer, SocketFlags.None, remoteEndPoint);

                    var actualSenderEndPoint = result.RemoteEndPoint;
                    ReadOnlySpan<byte> receivedData = new ReadOnlySpan<byte>(buffer, 0, result.ReceivedBytes);

                    ProcessPacket(receivedData, actualSenderEndPoint);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("UDP-сервер останавливается.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Произошла ошибка при работе UDP-сервера.");
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _socket?.Dispose();
            return base.StopAsync(cancellationToken);
        }

        private void ProcessPacket(ReadOnlySpan<byte> data, EndPoint senderEndPoint)
        {
            if (data.Length < 13) return;

            uint udpToken = BinaryPrimitives.ReadUInt32LittleEndian(data.Slice(0, 4));
            byte type = data[4];

            if (!_clientsRepository.tryGetByUdpToken(udpToken, out var clientConnection)) return;

            clientConnection.ConnectionEndPoint = senderEndPoint;

            var room = clientConnection.Session.CurrentRoom;

            if(room == null) return;

            var payload = data.Slice(5);
            var playerId = clientConnection.UserId;



            room.Router.Route(room,playerId, udpToken, type,payload);
        }
    }
}
