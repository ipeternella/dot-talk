using System;
using System.Collections.Generic;

namespace Dottalk.App.DTOs
{
    // Summary:
    //   DTO which contains all connected users of a specific chat node.
    public class NodeUser
    {
        public Guid NodeId;
        public IEnumerable<Guid> ConnectedUsers;
    }

    // Summary:
    //   DTO which contains all nodes that are currently hosting a chat room.
    public class ChatRoomConnections
    {
        public int TotalActiveConnections;
        public IEnumerable<NodeUser> NodeUsers;
    }
}