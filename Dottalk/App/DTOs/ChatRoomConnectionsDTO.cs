using System;
using System.Collections.Generic;

namespace Dottalk.App.DTOs
{
    // Summary:
    //   DTO which contains all connected users of a specific chat node.
    public class NodeUserDTO
    {
        public Guid NodeId;
        public IEnumerable<Guid> ConnectedUsers;
    }

    // Summary:
    //   DTO which contains all nodes that are currently hosting a chat room.
    public class ChatRoomConnectionsDTO
    {
        public int TotalActiveConnections;
        public IEnumerable<NodeUserDTO> NodeUsers;
    }
}