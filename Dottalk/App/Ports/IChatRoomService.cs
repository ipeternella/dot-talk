using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dottalk.App.Domain.Models;
using Dottalk.App.DTOs;
using Dottalk.App.Utils;

namespace Dottalk.App.Ports
{
    //
    // Summary:
    //   Defines the main usecases of the chat application that can be used.
    public interface IChatRoomService
    {
        Task<ChatRoomCreationResponseDTO> CreateChatRoom(ChatRoomCreationRequestDTO chatRoomCreationRequestDTO);

        Task<ChatRoomCreationResponseDTO> GetChatRoom(Guid chatRoomId);

        Task<IEnumerable<ChatRoomCreationResponseDTO>> GetAllChatRooms(PaginationParams? paginationParams);

        // Summary:
        //   Updates the active connections repository, freeing space for another user to join the room.
        // public ServiceResultVoidDTO RemoveConnection(Guid userId, Guid chatRoomId);

        // Summary:
        //   Processes commands from users, checking if there are commands, if the syntax is alright, etc. 
        //   in order to generate the final message sent to the users.
        // public ServiceResultDTO<string> ProcessMessage(string message, Guid userId, Guid chatRoomId);
    }
}