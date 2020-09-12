using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Dottalk.App.DTOs
{
    public class ChatRoomCreationRequestDTO
    {
        public string Name { get; set; } = default!;
        public int ActiveConnectionsLimit { get; set; }
    }

    public class ChatRoomCreationResponseDTO
    {
        public string Name { get; set; } = default!;
        public int ActiveConnectionsLimit { get; set; }
    }
}