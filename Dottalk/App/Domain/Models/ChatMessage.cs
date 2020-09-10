using System;
using System.ComponentModel.DataAnnotations;

namespace Dottalk.App.Domain.Models
{
    //
    // Summary:
    //   Represents a chat message sent by a user on a specific chat room.
    public class ChatMessage : BaseEntity
    {
        [Required]
        [MaxLength(255)]
        public string Message { get; set; } = default!;

        // Many-to-one User
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public User User { get; set; } = null!;

        // Many-to-one Chatroom
        [Required]
        public Guid ChatRoomId { get; set; }
        [Required]
        public ChatRoom ChatRoom { get; set; } = null!;
    }
}