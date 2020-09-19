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
        Task<ChatRoomResponseDTO> GetChatRoom(Guid chatRoomId);
        Task<ChatRoomResponseDTO> GetChatRoom(string chatRoomName);
        Task<IEnumerable<ChatRoomResponseDTO>> GetAllChatRooms(PaginationParams? paginationParams);
        Task<ChatRoomResponseDTO> CreateChatRoom(ChatRoomCreationRequestDTO chatRoomCreationRequestDTO);
        Task<ChatRoomConnectionPool> GetChatRoomConnectionPool(string chatRoomName);
        Task<ChatRoomConnectionPool> AddUserToChatRoomConnectionPool(string chatRoomName, string userName, string connectionId);
    }
}