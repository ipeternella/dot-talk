using System;
using System.Collections.Generic;
using System.Linq;
using Dottalk.App.Exceptions;

namespace Dottalk.App.Domain.Models
{
    //
    // Summary:
    //   All domain/business rules and logic related to chat rooms such as adding new connections, removing, etc.
    //
    // Raises:
    //   ChatRoomIsFullException - raised when the target chat room is already full
    //   UserIsAlreadyConnectedException - raised when if the user is already connected to the target chat room
    public class ChatRoomLogic
    {
        public static ChatRoomConnectionPool IncrementChatRoomConnectionPool(Guid userId,
            Guid serverInstanceId, string connectionId, ChatRoomConnectionPool chatRoomConnectionPool)
        {
            if (chatRoomConnectionPool.ActiveConnectionsLimit == chatRoomConnectionPool.TotalActiveConnections)
                throw new ChatRoomIsFullException("The chat room is already full.");

            var serverInstance = chatRoomConnectionPool.ServerInstances
                .Where(server => server.ServerInstanceId == serverInstanceId)
                .FirstOrDefault();

            // server instance hosting this chat room is new
            if (serverInstance == null)
            {
                chatRoomConnectionPool.ServerInstances = CreateFirstServerInstance(userId, serverInstanceId, connectionId);
                chatRoomConnectionPool.TotalActiveConnections += 1;

                return chatRoomConnectionPool;
            }

            // server instance is already hosting users for this room
            if (IsUserAlreadyInTheRoom(userId, serverInstance.ConnectedUsers))
                throw new UserIsAlreadyConnectedException($"User with id {userId} is already on the room!");

            serverInstance.ConnectedUsers = serverInstance.ConnectedUsers
                .AsQueryable().Append(new ConnectedUser { UserId = userId, ConnectionId = connectionId });

            chatRoomConnectionPool.TotalActiveConnections += 1;
            return chatRoomConnectionPool;
        }

        private static IEnumerable<ServerInstance> CreateFirstServerInstance(Guid userId, Guid serverInstanceId, string connectionId)
        {
            var connectedUsers = new List<ConnectedUser>() { new ConnectedUser { UserId = userId, ConnectionId = connectionId } };
            var serverInstance = new ServerInstance { ServerInstanceId = serverInstanceId, ConnectedUsers = connectedUsers };

            return new List<ServerInstance>() { serverInstance };
        }

        private static bool IsUserAlreadyInTheRoom(Guid userId, IEnumerable<ConnectedUser> connectedUsers)
        {
            var userAlreadyInTheRoom = connectedUsers.Where(conn => conn.UserId == userId).FirstOrDefault();

            return userAlreadyInTheRoom != null;
        }
    }
}