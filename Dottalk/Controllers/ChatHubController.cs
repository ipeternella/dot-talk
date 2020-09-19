using System;
using System.Threading.Tasks;
using Dottalk.App.Exceptions;
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
            var userConnectionId = Context.ConnectionId;
            _logger.LogInformation("A new user {userName:l} want to join room {chatRoomName:l} with connection id: {userConnectionId:l}",
                userName, chatRoomName, userConnectionId);

            try
            {
                _logger.LogInformation("Fetching chat room connection pool to see if it's full...");
                await _chatRoomService.AddUserToChatRoomConnectionPool(chatRoomName, userName, userConnectionId);
            }
            catch (Exception e) when
                (e is ObjectDoesNotExistException || e is ChatRoomIsFullException || e is UserIsAlreadyConnectedException)
            {
                throw new HubException(e.Message); // TODO: improve safety of the exception message with specific handler
            }

            _logger.LogInformation("Adding user to chat room group...");
            await Groups.AddToGroupAsync(Context.ConnectionId, chatRoomName);

            _logger.LogInformation("Broadcasting new user in room message...");
            await BroadcastMessageToChatRoom(chatRoomName, $"A new user has joined the room: {userName}");
            await base.OnConnectedAsync();
        }
        //
        // Summary:
        //   Attempts to receive a request to leave room.
        public async Task LeaveChatRoom(string chatRoomName)
        {
            var userConnectionId = Context.ConnectionId;
            _logger.LogInformation("User with connection {userConnectionId:l} want leave room {chatRoomName:l}", userConnectionId, chatRoomName);

            try
            {
                _logger.LogInformation("Removing user connection from redis connection pool...");
                await _chatRoomService.RemoveUserFromChatRoomConnectionPool(chatRoomName, userConnectionId);
            }
            catch (Exception e) when
                (e is ObjectDoesNotExistException || e is ChatRoomIsFullException || e is UserIsAlreadyConnectedException)
            {
                throw new HubException(e.Message); // TODO: improve safety of the exception message with specific handler
            }

            _logger.LogInformation("Removing user connection from in-memory connection pool.");
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