using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dottalk.App.Domain.Models;
using Dottalk.Infra.Persistence;

namespace Tests.Dottalk.Support
{
    public class TestingScenarioBuilder
    {
        public TestingScenarioBuilder()
        {

        }

        public static ChatRoomConnectionPool BuildChatRoomConnectionPoolEmpty(Guid chatRoomId)
        {
            var emptyChatConnectionPool = new ChatRoomConnectionPool()
            {
                ChatRoomId = chatRoomId,
                TotalActiveConnections = 0,  // no users
                ActiveConnectionsLimit = 6,
                ServerInstances = new List<ServerInstance>()  // no servers hosting this room
            };

            return emptyChatConnectionPool;
        }

        public static ChatRoomConnectionPool BuildChatRoomConnectionPoolTwoInstances(Guid chatRoomId, Guid instanceId1, Guid instanceId2)
        {
            var serverInstanceId1 = instanceId1;
            var serverInstanceId2 = instanceId2;

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

            var chatRoomActiveConnectionPool = new ChatRoomConnectionPool()
            {
                ChatRoomId = chatRoomId,
                TotalActiveConnections = 4,
                ActiveConnectionsLimit = 6,
                ServerInstances = new List<ServerInstance>() { serverInstance1, serverInstance2 }
            };

            return chatRoomActiveConnectionPool;
        }

        public static ChatRoomConnectionPool BuildChatRoomConnectionPoolWithFourUsers()
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

            var chatRoomActiveConnectionPool = new ChatRoomConnectionPool()
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

        public async static Task<Tuple<ChatRoom, User, ChatRoomConnectionPool>> BuildScenarioWithChatRoomAndUserWithPreviousConnectionPool(string chatRoomName,
            int activeConnectionsLimit, string userName, DBContext db, RedisContext redis)
        {
            var chatRoomConnectionPool = BuildChatRoomConnectionPoolWithFourUsers();
            var newChatRoom = new ChatRoom
            {
                Id = chatRoomConnectionPool.ChatRoomId,
                Name = chatRoomName,
                ActiveConnectionsLimit = activeConnectionsLimit
            };

            var newUser = new User
            {
                Name = userName
            };

            // creates chat room with the same Id of the pool and a user
            await db.ChatRooms.AddAsync(newChatRoom);
            await db.Users.AddAsync(newUser);
            await db.SaveChangesAsync();

            // creates a connection pool for the chat room
            await redis.SetKey<ChatRoomConnectionPool>(chatRoomConnectionPool.ChatRoomId, chatRoomConnectionPool, null);

            return new Tuple<ChatRoom, User, ChatRoomConnectionPool>(newChatRoom, newUser, chatRoomConnectionPool);
        }
    }
}