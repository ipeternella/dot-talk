using System.Threading.Tasks;
using Dottalk.App.DTOs;
using Dottalk.App.Ports;
using Microsoft.Extensions.DependencyInjection;
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
    }
}