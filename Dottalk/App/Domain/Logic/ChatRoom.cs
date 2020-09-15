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
        public static ChatRoomConnectionPool IncrementChatRoomConnectionPool(Guid userId,
            Guid serverInstanceId, ChatRoomConnectionPool chatRoomConnectionPool)
        {
            if (chatRoomConnectionPool.ActiveConnectionsLimit == chatRoomConnectionPool.TotalActiveConnections)
                throw new ChatRoomIsFullException("The chat room is already full.");

            var serverInstance = chatRoomConnectionPool.ServerInstances
                .Where(server => server.ServerInstanceId == serverInstanceId)
                .FirstOrDefault();

            if (serverInstance == null)  // server instance hosting the chat is new
            {
                var connectedUsers = new List<Guid>() { userId };  // list with a single user
                serverInstance = new ServerInstance { ServerInstanceId = serverInstanceId, ConnectedUsers = connectedUsers };
                chatRoomConnectionPool.ServerInstances = new List<ServerInstance>() { serverInstance };
            }
            else  // server instance hosting the chat already exists
            {
                serverInstance.ConnectedUsers = serverInstance.ConnectedUsers.AsQueryable().Append(userId);
            }

            chatRoomConnectionPool.TotalActiveConnections += 1;
            return chatRoomConnectionPool;
        }
    }
}