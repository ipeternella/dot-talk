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
            var chatRoomConnection = TestingScenarioBuilder.BuildChatRoomConnectionsWithFourUsers();
            var chatRoomId = Guid.NewGuid();
            var redis = ServiceProvider.GetRequiredService<RedisContext>();

            // act
            await redis.SetKey(chatRoomId, chatRoomConnection, TimeSpan.FromMinutes(1));

            // assert
            var persistedChatRoomConnections = await redis.GetKey<ChatRoomConnections>(chatRoomId);

            Assert.Equal(6, persistedChatRoomConnections.ActiveConnectionsLimit);
            Assert.Equal(4, persistedChatRoomConnections.TotalActiveConnections);
            Assert.Equal(2, persistedChatRoomConnections.ServerInstances.Count());
            Assert.Equal(chatRoomConnection.ServerInstances.ElementAt(1).ServerInstanceId.ToString(),
                persistedChatRoomConnections.ServerInstances.ElementAt(1).ServerInstanceId.ToString());
        }

        [Fact(DisplayName = "Should return a null value from Redis when the key is not found")]
        public async Task TestShouldRetunNullFromRedisWhenKeyIsNotFound()
        {
            // arrange
            var chatRoomConnection = TestingScenarioBuilder.BuildChatRoomConnectionsWithFourUsers();
            var redis = ServiceProvider.GetRequiredService<RedisContext>();

            // act
            await redis.SetKey("someKey", chatRoomConnection, null);

            // assert
            var nonExistentChatRoom = await redis.GetKey<ChatRoomConnections>("anotherKey");
            var existentChatRoom = await redis.GetKey<ChatRoomConnections>("someKey");

            Assert.Null(nonExistentChatRoom);
            Assert.NotNull(existentChatRoom);
            Assert.Equal(chatRoomConnection.ChatRoomId, existentChatRoom.ChatRoomId);
        }
    }
}