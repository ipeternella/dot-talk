using AutoMapper;
using Dottalk.App.Domain.Models;

namespace Dottalk.App.DTOs.Mapping
{
    //
    // Summary:
    //   Mappings used to convert from DTOs -> domain.
    public class DTO2Domain : Profile
    {
        public DTO2Domain()
        {
            CreateMap<ChatRoomCreationRequestDTO, ChatRoom>();
            CreateMap<UserCreationRequestDTO, User>();
        }
    }
}