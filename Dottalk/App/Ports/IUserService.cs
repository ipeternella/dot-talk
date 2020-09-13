using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dottalk.App.DTOs;
using Dottalk.App.Utils;

namespace Dottalk.App.Ports
{
    //
    // Summary:
    //   Defines the main usecases related to users of the application.
    public interface IUserService
    {
        Task<UserResponseDTO> GetUser(Guid userId);
        Task<UserResponseDTO> GetUser(string userName);
        Task<IEnumerable<UserResponseDTO>> GetAllUsers(PaginationParams? paginationParams);
        Task<UserResponseDTO> CreateUser(UserCreationRequestDTO userCreationRequestDTO);
    }
}