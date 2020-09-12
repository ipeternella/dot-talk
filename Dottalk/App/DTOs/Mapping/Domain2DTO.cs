using AutoMapper;
using Dottalk.App.Domain.Models;

namespace Dottalk.App.DTOs.Mapping
{
    //
    // Summary:
    //   Mappings used to convert from Domain models -> DTOs.
    public class Domain2DTO : Profile
    {
        public Domain2DTO()
        {
            CreateMap<ChatRoom, ChatRoomCreationResponseDTO>();
        }
    }
}