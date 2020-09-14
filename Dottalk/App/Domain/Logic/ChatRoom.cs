using System;
using System.Collections.Generic;
using System.Linq;

namespace Dottalk.App.Domain.Models
{
    //
    // Summary:
    //   All domain/business rules and logic related to chat rooms such as adding new connections, removing, etc.
    public class ChatRoomLogic
    {
        public static ChatRoomActiveConnectionPool IncrementChatRoomConnections(Guid userId,
            Guid serverInstanceId, ChatRoomActiveConnectionPool chatRoomConnectionPool)
        {
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