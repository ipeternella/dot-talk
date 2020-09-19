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

        public static ChatRoomConnectionPool BuildChatRoomConnectionPoolTwoInstances(Guid chatRoomId, int activeConnectionsLimit, Guid instanceId1, Guid instanceId2)
        {
            var connectedUserId1 = Guid.NewGuid();
            var connectedUserId2 = Guid.NewGuid();
            var connectedUserId3 = Guid.NewGuid();
            var connectedUserId4 = Guid.NewGuid();

            var connectedUser1 = new ConnectedUser
            {
                UserId = connectedUserId1,
                ConnectionId = "connection 1"
            };

            var connectedUser2 = new ConnectedUser
            {
                UserId = connectedUserId2,
                ConnectionId = "connection 2"
            };

            var connectedUser3 = new ConnectedUser
            {
                UserId = connectedUserId3,
                ConnectionId = "connection 3"
            };

            var connectedUser4 = new ConnectedUser
            {
                UserId = connectedUserId4,
                ConnectionId = "connection 4"
            };

            var serverInstance1 = new ServerInstance()
            {
                ServerInstanceId = instanceId1,
                ConnectedUsers = new List<ConnectedUser>() { connectedUser1, connectedUser2 }
            };

            var serverInstance2 = new ServerInstance()
            {
                ServerInstanceId = instanceId2,
                ConnectedUsers = new List<ConnectedUser>() { connectedUser3, connectedUser4 }
            };

            var chatRoomConnectionPool = new ChatRoomConnectionPool()
            {
                ChatRoomId = chatRoomId,
                ActiveConnectionsLimit = activeConnectionsLimit,
                TotalActiveConnections = 4,
                ServerInstances = new List<ServerInstance>() { serverInstance1, serverInstance2 }
            };

            return chatRoomConnectionPool;
        }

        public static ChatRoomConnectionPool BuildChatRoomConnectionPoolWithFourUsers()
        {
            var chatRoomId = Guid.NewGuid();
            var instanceId1 = Guid.NewGuid();
            var instanceId2 = Guid.NewGuid();
            var activeConnectionsLimit = 6;

            return BuildChatRoomConnectionPoolTwoInstances(chatRoomId, activeConnectionsLimit, instanceId1, instanceId2);
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
            await redis.SetKey(chatRoomConnectionPool.ChatRoomId, chatRoomConnectionPool, null);

            return new Tuple<ChatRoom, User, ChatRoomConnectionPool>(newChatRoom, newUser, chatRoomConnectionPool);
        }
    }
}