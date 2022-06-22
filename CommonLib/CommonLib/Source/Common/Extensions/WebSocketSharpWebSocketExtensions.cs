using System;
using System.Reflection;
using WebSocketSharp;

namespace CommonLib.Source.Common.Extensions
{
    public static class WebSocketSharpWebSocketExtensions
    {
        public static string Id(this WebSocket socket)
        {
            if (socket == null)
                throw new ArgumentNullException(nameof(socket));

            return socket.GetType().GetField("_base64Key", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(socket).ToString();
        }

        public static WebSocket Reconnect(this WebSocket socket, bool force = false)
        {
            if (socket == null)
                throw new ArgumentNullException(nameof(socket));

            if (socket.ReadyState != WebSocketState.Open)
                socket.Connect();
            else if (force && socket.ReadyState == WebSocketState.Open)
            {
                socket.Close();
                socket.Connect();
            }
            return socket;
        }
    }
}
