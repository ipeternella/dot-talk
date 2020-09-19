using System;
using System.Collections.Generic;
using System.Linq;
using Dottalk.App.Exceptions;

namespace Dottalk.App.Domain.Models
{
    //
    // Summary:
    //   All domain/business rules and logic related to chat rooms such as adding new connections, removing, etc.
    public class ChatRoomLogic
    {
        //
        //  Summary:
        //    Decrements a chat room's connection pool on Redis.
        //
        //  Exceptions
        //    UserIsNotConnectedException - raised if the user connection was not found.
        public static ChatRoomConnectionPool DecrementChatRoomConnectionPool(Guid serverInstanceId,
            string connectionId, ChatRoomConnectionPool chatRoomConnectionPool)
        {
            var serverInstance = chatRoomConnectionPool.ServerInstances
                .Where(server => server.ServerInstanceId == serverInstanceId)
                .FirstOrDefault();
            var connectedUser = serverInstance?.ConnectedUsers.Where(conn => conn.ConnectionId == connectionId);

            // makes sure a server instance exists and that the connectionId exists
            if (serverInstance == null || connectedUser == null)
                throw new ObjectDoesNotExistException("User was not found in the room! Maybe he has left already?");

            // updated list with the removed connection
            var updatedConnections = serverInstance.ConnectedUsers.ToList();
            updatedConnections.RemoveAll(conn => conn.ConnectionId == connectionId);

            serverInstance.ConnectedUsers = updatedConnections;
            chatRoomConnectionPool.TotalActiveConnections -= 1;

            return chatRoomConnectionPool;
        }
        //
        //  Summary:
        //    Increments a chat room's connection pool on Redis.
        //
        //  Exceptions
        //    ChatRoomIsFullException - raised when the chat room is full
        //    UserIsAlreadyConnectedException - raised if the user is already connected to the room
        public static ChatRoomConnectionPool IncrementChatRoomConnectionPool(Guid userId,
            Guid serverInstanceId, string connectionId, ChatRoomConnectionPool chatRoomConnectionPool)
        {
            if (chatRoomConnectionPool.ActiveConnectionsLimit == chatRoomConnectionPool.TotalActiveConnections)
                throw new ChatRoomIsFullException("The chat room is already full!");

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
                throw new UserIsAlreadyConnectedException("User is already in the room!");

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