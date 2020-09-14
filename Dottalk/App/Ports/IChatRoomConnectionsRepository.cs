using System;
using Dottalk.App.Domain.Models;

namespace Dottalk.App.Ports
{
    //
    // Summary:
    //   Interface used to interact with a memory cache/Redis in order to retrieve all hosting nodes
    //   that are hosting a chat room. Works closely with the ChatRoomConnectionPool DTO.
    public interface IChatRoomConnectionPoolRepository
    {
        public void AddChatRoomConnection(Guid chatRoomId, Guid userId, Guid ServerInstanceId);
        public void RemoveChatRoomConnection(Guid chatRoomId, Guid userId, Guid ServerInstanceId);
        public int TotalActiveConnections(Guid chatRoomId);
        public ChatRoomConnectionPool GetChatRoomConnectionPool(Guid chatRoomId);
    }
}