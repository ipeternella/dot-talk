using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Dottalk.App.DTOs
{
    public class ChatRoomCreationDTO
    {
        public string ChatRoomName { get; set; } = default!;
        public int ActiveConnectionsLimit { get; set; }
    }
}