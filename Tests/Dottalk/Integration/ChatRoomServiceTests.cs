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
            var newChatRoomRequest = new ChatRoomCreationRequestDTO
            {
                Name = "New chat room",
                ActiveConnectionsLimit = 4
            };
            var chatRoomService = ServiceProvider.GetRequiredService<IChatRoomService>();

            // act
            var result = await chatRoomService.CreateChatRoom(newChatRoomRequest);

            // assert
            var persistedChatRoom = await DB.ChatRooms.FindAsync(result.Id);

            Assert.Equal(newChatRoomRequest.Name, persistedChatRoom.Name);
            Assert.Equal(newChatRoomRequest.ActiveConnectionsLimit, persistedChatRoom.ActiveConnectionsLimit);
        }
    }
}