using System;
using System.Threading.Tasks;
using Dottalk.App.Domain.Models;
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
        public async Task JoinChatRoom(string chatRoomName, string userName)
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
            await base.OnConnectedAsync();

            await BroadcastSystemMessageToChatRoom($"A new user has joined the room: {userName}", chatRoomName);
        }
        //
        // Summary:
        //   Attempts to receive a request to leave room.
        public async Task LeaveChatRoom(string chatRoomName, string userName)
        {
            _logger.LogInformation("User {userName:l} want leave room {chatRoomName:l}", userName, chatRoomName);

            try
            {
                var (updatedConnectionPool, connectionId) = await _chatRoomService.RemoveUserFromChatRoomConnectionPool(chatRoomName, userName);
                _logger.LogInformation("Removed user from connection pool: {@updatedConnectionPool}", updatedConnectionPool);

                _logger.LogInformation("Removing connection {connectionId:l} connection from in-memory connection pool.", connectionId);
                await Groups.RemoveFromGroupAsync(connectionId, chatRoomName);
            }
            catch (Exception e) when (e is ObjectDoesNotExistException || e is ChatRoomIsFullException || e is UserIsAlreadyConnectedException)
            {
                throw new HubException(e.Message); // TODO: improve safety of the exception message with specific handler
            }

            await base.OnDisconnectedAsync(null);
            await BroadcastSystemMessageToChatRoom($"User {userName} has left the room...", chatRoomName);
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
        //   Broadcasts a message to the whole chat room (system messages).
        public async Task BroadcastSystemMessageToChatRoom(string message, string chatRoomName)
        {
            var chatMessage = message;

            await Clients.Group(chatRoomName).SendAsync("ReceiveMessage", chatMessage);
        }
        //
        // Summary:
        //   Broadcasts a message that came from a user to the whole chat room.
        public async Task BroadcastUserMessageToChatRoom(string message, string chatRoomName, string userName)
        {
            try
            {
                var processedChatMessage = await _chatRoomService.ProcessUserMessage(userName, chatRoomName, message);
                await Clients.Group(chatRoomName).SendAsync("ReceiveMessage", processedChatMessage);
            }
            catch (Exception e) when (e is ObjectDoesNotExistException)
            {
                throw new HubException(e.Message); // TODO: improve safety of the exception message with specific handler
            }
        }
    }
}