using System;
using System.Collections.Generic;

namespace Dottalk.App.Domain.Models
{
    //
    // Summary:
    //   Represents all the connections that exist for a specific chat room (possibly hosted on more than one
    //   application instance.)
    //
    // Expected serialization:
    //   {
    //     "chatRoomId": "IGP",
    //     "totalActiveConnections": 5,  // total of 5 users connected to two different server instances
    //     "serverInstances": [
    //        {"serverInstanceId": "123", "connectedUsers": ["A", "B", "C"]}  // 3 users on this instance
    //        {"serverInstanceId": "456", "connectedUsers": ["F", "Z"]}       // 2 users on this instance
    //     ]
    //   }
    public class ChatRoomConnections
    {
        public Guid ChatRoomId;
        public int TotalActiveConnections;
        public IEnumerable<ServerInstance> ServerInstances;
    }
    //
    // Summary:
    //   Represents all the connected users that are connected to a specific application instance.
    public class ServerInstance
    {
        public Guid ServerInstanceId;
        public IEnumerable<Guid> ConnectedUsers;
    }
}