using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Dottalk.App.Domain.Models
{
    //
    // Summary:
    //   Represents a chat room where users can communicate and send messages.
    public class ChatRoom : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        [Range(2, 10)]
        public int ActiveConnectionsLimit { get; set; }

        // navigation property
        public ICollection<ChatMessage> ChatMessages { get; set; } = null!;
    }
}