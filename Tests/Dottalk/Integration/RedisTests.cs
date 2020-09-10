using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Dottalk.App.Domain.Models;
using Dottalk.Infra.Persistence;
using Microsoft.Extensions.Configuration;
using Tests.Dottalk.Support;
using Xunit;

namespace Tests.Dottalk.Unit
{
    public class RedisTests
    {
        private readonly RedisContext _redis;

        public RedisTests()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            _redis = new RedisContext(configuration);
        }

        [Fact(DisplayName = "Should save a chat connection and retrieve from Redis")]
        public async Task TestShouldSaveChatConnectionAndRetrieveFromRedis()
        {
            // arrange
            var chatRoomConnection = TestingScenarioBuilder.BuildChatRoomConnectionsWithFourUsers();
            var chatRoomId = Guid.NewGuid();

            // act
            await _redis.SetKey(chatRoomId, chatRoomConnection, TimeSpan.FromMinutes(1));
            var persistedChatRoomConnections = await _redis.GetKey<ChatRoomConnections>(chatRoomId);

            // assert
            Assert.Equal(6, persistedChatRoomConnections.ActiveConnectionsLimit);
            Assert.Equal(4, persistedChatRoomConnections.TotalActiveConnections);
            Assert.Equal(2, persistedChatRoomConnections.ServerInstances.Count());
            Assert.Equal(chatRoomConnection.ServerInstances.ElementAt(1).ServerInstanceId.ToString(),
                persistedChatRoomConnections.ServerInstances.ElementAt(1).ServerInstanceId.ToString());
        }
    }
}