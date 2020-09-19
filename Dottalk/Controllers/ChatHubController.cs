using System;
using System.Threading.Tasks;
using Dottalk.App.Ports;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Dottalk.Controllers
{
    //
    // Summary:
    //   Controller responsible for receiving new connections/disconnections and for processing messages and
    //   commands sent by the users. Based on RPC (remote procedure calls) due to SignalR.
    public class ChatHubController : Hub
    {
        private readonly ILogger<ChatHubController> _logger;
        private readonly IChatRoomService _chatRoomService;
        private readonly IUserService _userService;

        public ChatHubController(ILogger<ChatHubController> logger, IChatRoomService chatRoomService, IUserService userService)
        {
            _chatRoomService = chatRoomService;
            _userService = userService;
            _logger = logger;
        }
        //
        // Summary:
        //   Attempts to accept a new connection for a given chat room. Can fail if the room is full, for example.
        public async Task JoinChatRoom(string userName, string chatRoomName)
        {
            _logger.LogInformation("A new user {userName:l} want to join room {chatRoomName:l}", userName, chatRoomName);
            var chatRoom = await _chatRoomService.GetChatRoom(chatRoomName);
            var user = await _userService.GetUser(userName);
            var userConnectionId = Context.ConnectionId;

            _logger.LogInformation("Fetching chat room connection pool to see if it's full...");
            await _chatRoomService.AddUserToChatRoomConnectionPool(chatRoom.Name, user.Name, userConnectionId);

            _logger.LogInformation("Adding user to chat room group...");
            await Groups.AddToGroupAsync(Context.ConnectionId, chatRoom.Name);

            await BroadcastMessageToChatRoom(chatRoomName, $"A new user has joined the room: {user}");
            await base.OnConnectedAsync();
        }
        //
        // Summary:
        //   Attempts to receive a request to leave room.
        public async Task LeaveChatRoom(string chatRoomName)
        {
            // TODO: improve chat room connection pool to remove user by his connectionId
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, chatRoomName);
            await base.OnDisconnectedAsync(null);
        }
        //
        // Summary:
        //   Attempts to remove a connection from the Hub if there's a connection problem from the user.
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (exception == null) return;

            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "SignalR Users");
            await base.OnDisconnectedAsync(exception);
        }
        //
        // Summary:
        //   Broadcasts a message to the whole chat room.
        public async Task BroadcastMessageToChatRoom(string message, string chatRoomName)
        {
            await Clients.Group(chatRoomName).SendAsync("ReceiveMessage", message);
        }
    }
}