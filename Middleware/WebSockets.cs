using System;
using System.Net.WebSockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;

namespace Dottalk.Middleware
{
    // Summary:
    //   Repository for managing chat room connections that are sustained through several nodes.
    public class WebSocketController
    {
        public static async Task AcceptWebSocket(HttpContext context, Func<Task> next)
        {
            var webSocketRoutePattern = new Regex(@"^\/ws\/(?<chatRoomName>\w+)$", RegexOptions.IgnoreCase);
            var match = webSocketRoutePattern.Match(context.Request.Path);

            if (match.Success)
            {
                var chatRoomName = match.Groups["chatRoomName"].Value;

                if (context.WebSockets.IsWebSocketRequest)
                {
                    // check if room is exists/full/etc.
                    WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                }
                else
                {
                    context.Response.StatusCode = 400;
                }
            }
            else
            {
                await next();
            }
        }
    }
}