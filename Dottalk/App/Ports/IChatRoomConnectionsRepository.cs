using System;
using Dottalk.App.Domain.Models;

namespace Dottalk.App.Ports
{
    //
    // Summary:
    //   Interface used to interact with a memory cache/Redis in order to retrieve all hosting nodes
    //   that are hosting a chat room. Works closely with the ChatRoomActiveConnectionPool DTO.
    public interface IChatRoomActiveConnectionPoolRepository
    {
        public void AddChatRoomConnection(Guid chatRoomId, Guid userId, Guid ServerInstanceId);
        public void RemoveChatRoomConnection(Guid chatRoomId, Guid userId, Guid ServerInstanceId);
        public int TotalActiveConnections(Guid chatRoomId);
        public ChatRoomActiveConnectionPool GetChatRoomActiveConnectionPool(Guid chatRoomId);
    }
}