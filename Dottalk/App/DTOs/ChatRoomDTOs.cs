using System;
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
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public int ActiveConnectionsLimit { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}