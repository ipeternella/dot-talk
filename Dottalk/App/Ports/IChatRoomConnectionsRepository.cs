using System;
using Dottalk.App.DTOs;

namespace Dottalk.App.Ports
{
    // Summary:
    //   Interface used to interact with a memory cache/Redis in order to retrieve all hosting nodes
    //   that are hosting a chat room. Works closely with the ChatRoomConnections DTO.
    public interface IChatRoomConnectionsRepository
    {
        public void AddConnection(Guid chatRoomId, Guid userId, Guid nodeId);
        public void RemoveConnection(Guid chatRoomId, Guid userId, Guid nodeId);
        public int TotalActiveConnections(Guid chatRoomId);
        public ChatRoomConnectionsDTO GetChatRoomConnections(Guid chatRoomId);
    }
}