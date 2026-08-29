using System.Net.WebSockets;
using System.Text;

namespace GameServer.WebSockets
{
    public static class WebSocketExtensions
    {
        public static async Task SendTextAsync(this WebSocket socket, string message, CancellationToken ct = default)
        {
            var bytes = Encoding.UTF8.GetBytes(message);
            var segment = new ArraySegment<byte>(bytes);

            await socket.SendAsync(segment, WebSocketMessageType.Text, true, ct);
        }
    }
}
