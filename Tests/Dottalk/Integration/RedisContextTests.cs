using System;
using System.Linq;
using System.Threading.Tasks;
using Dottalk.App.Domain.Models;
using Dottalk.Infra.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Tests.Dottalk.Support;
using Tests.Hangman.Support;
using Xunit;

namespace Tests.Dottalk.Unit
{
    public class RedisContextTests : BaseTestCase<TestingStartUp>
    {
        [Fact(DisplayName = "Should save a chat connection and retrieve from Redis")]
        public async Task TestShouldSaveChatConnectionAndRetrieveFromRedis()
        {
            // arrange
            var chatRoomConnection = TestingScenarioBuilder.BuildChatRoomConnectionPoolWithFourUsers();
            var chatRoomId = Guid.NewGuid();
            var redis = ServiceProvider.GetRequiredService<RedisContext>();

            // act
            await redis.SetKey(chatRoomId, chatRoomConnection, TimeSpan.FromMinutes(1));

            // assert
            var persistedChatRoomConnectionPool = await redis.GetKey<ChatRoomConnectionPool>(chatRoomId);

            Assert.Equal(6, persistedChatRoomConnectionPool.ActiveConnectionsLimit);
            Assert.Equal(4, persistedChatRoomConnectionPool.TotalActiveConnections);
            Assert.Equal(2, persistedChatRoomConnectionPool.ServerInstances.Count());

            // first server instance
            Assert.Equal(chatRoomConnection.ServerInstances.ElementAt(0).ServerInstanceId.ToString(),
                persistedChatRoomConnectionPool.ServerInstances.ElementAt(0).ServerInstanceId.ToString());
            Assert.Equal(chatRoomConnection.ServerInstances.ElementAt(0).ConnectedUsers.First().UserId,
                persistedChatRoomConnectionPool.ServerInstances.ElementAt(0).ConnectedUsers.First().UserId);
            Assert.Equal("connection 1", chatRoomConnection.ServerInstances.ElementAt(0).ConnectedUsers.First().ConnectionId);
            Assert.Equal("connection 2", chatRoomConnection.ServerInstances.ElementAt(0).ConnectedUsers.ElementAt(1).ConnectionId);

            // second server instance hosting the chat room
            Assert.Equal(chatRoomConnection.ServerInstances.ElementAt(1).ServerInstanceId.ToString(),
                persistedChatRoomConnectionPool.ServerInstances.ElementAt(1).ServerInstanceId.ToString());
            Assert.Equal(chatRoomConnection.ServerInstances.ElementAt(1).ConnectedUsers.First().UserId,
                persistedChatRoomConnectionPool.ServerInstances.ElementAt(1).ConnectedUsers.First().UserId);
            Assert.Equal("connection 3", chatRoomConnection.ServerInstances.ElementAt(1).ConnectedUsers.First().ConnectionId);
            Assert.Equal("connection 4", chatRoomConnection.ServerInstances.ElementAt(1).ConnectedUsers.ElementAt(1).ConnectionId);
        }

        [Fact(DisplayName = "Should return a null value from Redis when the key is not found")]
        public async Task TestShouldRetunNullFromRedisWhenKeyIsNotFound()
        {
            // arrange
            var chatRoomConnection = TestingScenarioBuilder.BuildChatRoomConnectionPoolWithFourUsers();
            var redis = ServiceProvider.GetRequiredService<RedisContext>();

            // act
            await redis.SetKey("someKey", chatRoomConnection, null);

            // assert
            var nonExistentChatRoom = await redis.GetKey<ChatRoomConnectionPool>("anotherKey");
            var existentChatRoom = await redis.GetKey<ChatRoomConnectionPool>("someKey");

            Assert.Null(nonExistentChatRoom);
            Assert.NotNull(existentChatRoom);
            Assert.Equal(chatRoomConnection.ChatRoomId, existentChatRoom.ChatRoomId);
        }
    }
}