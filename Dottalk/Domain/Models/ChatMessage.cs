using System;

namespace Dottalk.Domain.Models
{
    public class ChatMessage
    {
        public Guid UserId;
        public Guid ChatRoomId;
        public string Message;
    }
}