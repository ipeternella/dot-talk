using System;
using System.Collections.Generic;
using Dottalk.App.Domain.Models;
using Dottalk.Infra.Persistence;

namespace Tests.Dottalk.Support
{
    public class TestingScenarioBuilder
    {
        public TestingScenarioBuilder()
        {

        }

        public static ChatRoomActiveConnectionPool BuildChatRoomActiveConnectionPoolWithFourUsers()
        {
            var chatRoomId = Guid.NewGuid();
            var serverInstanceId1 = Guid.NewGuid();
            var serverInstanceId2 = Guid.NewGuid();
            var connectedUserId1 = Guid.NewGuid();
            var connectedUserId2 = Guid.NewGuid();
            var connectedUserId3 = Guid.NewGuid();
            var connectedUserId4 = Guid.NewGuid();

            var serverInstance1 = new ServerInstance()
            {
                ServerInstanceId = serverInstanceId1,
                ConnectedUsers = new List<Guid>() { connectedUserId1, connectedUserId2 }
            };

            var serverInstance2 = new ServerInstance()
            {
                ServerInstanceId = serverInstanceId2,
                ConnectedUsers = new List<Guid>() { connectedUserId3, connectedUserId4 }
            };

            var chatRoomActiveConnectionPool = new ChatRoomActiveConnectionPool()
            {
                ChatRoomId = chatRoomId,
                TotalActiveConnections = 4,
                ActiveConnectionsLimit = 6,
                ServerInstances = new List<ServerInstance>() { serverInstance1, serverInstance2 }
            };

            return chatRoomActiveConnectionPool;
        }

        public static Tuple<ChatRoom, User> BuildScenarioWithChatRoomAndUser(string chatRoomName,
            int activeConnectionsLimit, string userName, DBContext db)
        {
            var newChatRoom = new ChatRoom
            {
                Name = chatRoomName,
                ActiveConnectionsLimit = activeConnectionsLimit
            };

            var newUser = new User
            {
                Name = userName
            };

            db.ChatRooms.Add(newChatRoom);
            db.Users.Add(newUser);
            db.SaveChanges();

            return new Tuple<ChatRoom, User>(newChatRoom, newUser);
        }
    }
}