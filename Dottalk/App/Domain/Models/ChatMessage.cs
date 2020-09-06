using System;

namespace Dottalk.App.Domain.Models
{
    public class ChatMessage
    {
        public Guid UserId;
        public Guid ChatRoomId;
        public string Message;
    }
}