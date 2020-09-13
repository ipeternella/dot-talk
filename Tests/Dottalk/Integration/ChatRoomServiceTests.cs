using System;
using System.Linq;
using System.Threading.Tasks;
using Dottalk.App.DTOs;
using Dottalk.App.Ports;
using Microsoft.Extensions.DependencyInjection;
using Tests.Dottalk.Support;
using Tests.Hangman.Support;
using Xunit;

namespace Tests.Dottalk.Integration
{
    public class ChatRoomServiceTests : BaseTestCase<TestingStartUp>
    {
        [Fact(DisplayName = "Chat room service should create and retrive a new chat room")]
        public async Task TestChatRoomServiceShouldCreateAndRetrieveNewChatRoom()
        {
            // arrange
            var chatRoomService = ServiceProvider.GetRequiredService<IChatRoomService>();
            var newChatRoomRequest = new ChatRoomCreationRequestDTO
            {
                Name = "New chat room",
                ActiveConnectionsLimit = 4
            };

            // act
            var result = await chatRoomService.CreateChatRoom(newChatRoomRequest);

            // assert
            var persistedChatRoom = await DB.ChatRooms.FindAsync(result.Id);  // from db
            var serviceResult = await chatRoomService.GetChatRoom(result.Id);  // from chat service

            Assert.Equal(newChatRoomRequest.Name, persistedChatRoom.Name);
            Assert.Equal(newChatRoomRequest.ActiveConnectionsLimit, persistedChatRoom.ActiveConnectionsLimit);

            Assert.Equal(newChatRoomRequest.Name, serviceResult.Name);
            Assert.Equal(newChatRoomRequest.ActiveConnectionsLimit, serviceResult.ActiveConnectionsLimit);
            Assert.Equal(persistedChatRoom.Id, serviceResult.Id);
        }

        [Fact(DisplayName = "Chat room service should get a connection pool of a chat room for the first time")]
        public async Task TestChatRoomShouldGetConnectionPoolOfChatRoomForFirstTime()
        {
            // arrange
            var chatRoomService = ServiceProvider.GetRequiredService<IChatRoomService>();
            var (chatRoom, _) = TestingScenarioBuilder.BuildScenarioWithChatRoomAndUser("Chat 1", 4, "IGP", DB);

            // act
            var chatActiveConnectionPool = await chatRoomService.GetOrCreateChatRoomActiveConnectionPool(chatRoom.Name);

            // assert
            Assert.Equal(chatRoom.Id, chatActiveConnectionPool.ChatRoomId);
            Assert.Equal(4, chatRoom.ActiveConnectionsLimit);
            Assert.Equal(0, chatActiveConnectionPool.TotalActiveConnections);  // no connections for the first time
            Assert.Empty(chatActiveConnectionPool.ServerInstances);
        }

        [Fact(DisplayName = "Chat room service should get a previous connection pool of a chat room")]
        public async Task TestChatRoomShouldGetPreviousConnectionPoolOfChatRoom()
        {
            // arrange
            var chatRoomService = ServiceProvider.GetRequiredService<IChatRoomService>();
            var (chatRoom, _, chatActiveConnectionPool) = await TestingScenarioBuilder
                .BuildScenarioWithChatRoomAndUserWithPreviousConnectionPool("Chat 1", 4, "IGP", DB, Redis);

            // act
            var createdChatActiveConnectionPool = await chatRoomService.GetOrCreateChatRoomActiveConnectionPool(chatRoom.Name);

            // assert
            Assert.Equal(chatRoom.Id, createdChatActiveConnectionPool.ChatRoomId);
            Assert.Equal(4, chatRoom.ActiveConnectionsLimit);
            Assert.Equal(4, createdChatActiveConnectionPool.TotalActiveConnections);  // 4 active connections
            Assert.Equal(2, createdChatActiveConnectionPool.ServerInstances.Count());
            Assert.Equal(2, createdChatActiveConnectionPool.ServerInstances.ElementAt(1).ConnectedUsers.Count());
            Assert.Equal(chatActiveConnectionPool.ServerInstances.ElementAt(1).ServerInstanceId,
                createdChatActiveConnectionPool.ServerInstances.ElementAt(1).ServerInstanceId);
        }
    }
}