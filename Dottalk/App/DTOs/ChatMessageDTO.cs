using System;

namespace Dottalk.App.DTOs
{
    public class ChatMessageDTO
    {
        public Guid UserId;
        public string Message = default!;
    }
}