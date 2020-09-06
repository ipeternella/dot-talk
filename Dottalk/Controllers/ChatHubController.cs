using System;
using System.Threading.Tasks;
using Dottalk.App.Ports;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Dottalk.Controllers
{
    // Summary:
    //   Controller/presenter responsible for receiving new connections/disconnections and for processing messages and
    //   commands sent by the users. Based on RPC (remote procedure calls) due to SignalR.
    public class ChatHubController : Hub
    {
        private readonly ILogger<ChatHubController> _logger;
        private readonly IChatRoomService _chatRoomService;

        public ChatHubController(ILogger<ChatHubController> logger, IChatRoomService chatRoomService)
        {
            _chatRoomService = chatRoomService;
            _logger = logger;
        }

        // Summary:
        //   Attempts to accept a new connection for a given chat room. Can fail if the room is full, for example.
        public async Task JoinRoom(string user, string roomName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
            await SendMessage(user, $"A new user has joined the room: {user}");
            await base.OnConnectedAsync();
        }

        // Summary:
        //   Attempts to receive a request to leave room.
        public async Task LeaveRoom(string roomName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
            await base.OnDisconnectedAsync(null);
        }

        // Summary:
        //   Attempts to remove a connection from the Hub if there's a connection problem from the user.
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (exception == null) return;

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SignalR Users");
            await base.OnDisconnectedAsync(exception);
        }

        // Summary:
        //   Receives messages from users, process them with the chatting services and then returns the processed message
        //   to the users. Checks for commands, if the command syntax is alright, etc.
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}