using System;
using Dottalk.App.DTOs;

namespace Dottalk.App.Ports
{
    // Summary:
    //   Defines the main usecases of the chat application that can be used.
    public interface IChatRoomService
    {
        // Summary:
        //   Asserts that the room is not full and updates the active connections repository.
        public ServiceResultVoidDTO AddConnection(Guid userId, Guid chatRoomId);

        // Summary:
        //   Updates the active connections repository, freeing space for another user to join the room.
        // public ServiceResultVoidDTO RemoveConnection(Guid userId, Guid chatRoomId);

        // Summary:
        //   Processes commands from users, checking if there are commands, if the syntax is alright, etc. 
        //   in order to generate the final message sent to the users.
        // public ServiceResultDTO<string> ProcessMessage(string message, Guid userId, Guid chatRoomId);
    }
}