using System;
using System.ComponentModel.DataAnnotations;

namespace Dottalk.App.Domain.Models
{
    //
    // Summary:
    //   Represents a chat message sent by a user on a specific chat room.
    public class ChatMessage : BaseEntity
    {
        public User User { get; set; } = null!;
        public ChatRoom ChatRoom { get; set; } = null!;
        [MaxLength(255)]
        public string Message { get; set; } = null!;
    }
}