using System.Threading.Tasks;
using Dottalk.App.DTOs;
using Tests.Hangman.Support;
using Xunit;

namespace Tests.Dottalk.Integration
{
    public class ChatRoomServiceTests : BaseTestCase
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

            // act
            var result = await ChatRoomService.CreateChatRoom(newChatRoomRequest);

            // assert
            var persistedChatRoom = await DB.ChatRooms.FindAsync(result.Id);

            Assert.Equal(newChatRoomRequest.Name, persistedChatRoom.Name);
            Assert.Equal(newChatRoomRequest.ActiveConnectionsLimit, persistedChatRoom.ActiveConnectionsLimit);
        }
    }
}