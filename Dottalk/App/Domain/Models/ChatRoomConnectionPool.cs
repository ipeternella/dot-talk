using System;
using System.Collections.Generic;

namespace Dottalk.App.Domain.Models
{
    //
    // Summary:
    //   Represents all the connections that exist for a specific chat room (possibly hosted on more than one
    //   application instance).
    //
    // Expected serialization:
    //
    // {
    //     "chatRoomId": "6576b56b-8c96-47d0-b17f-70ef3d84bfda",
    //     "activeConnectionsLimit": 6,
    //     "totalActiveConnections": 5,
    //     "serverInstances": [
    //         {   
    //             "serverInstanceId": "4945c63c-0e8b-4fb2-810b-5c8071091e04", 
    //             "connectedUsers": [
    //                 {
    //                    "UserId":"1c101959-b95e-4c08-bd69-a2e1ee759646",
    //                    "ConnectionId" :"connection 1"
    //                 },
    //                 {
    //                    "UserId":"8e9404e6-ba38-4118-9a19-0060314be702",
    //                    "ConnectionId" :"connection 2"
    //                 },
    //                 {
    //                    "UserId":"1f5ee50d-6e78-4ed7-bc65-4b69ada9de8a",
    //                    "ConnectionId" :"connection 3"
    //                 }
    //             ]
    //         },
    //         {
    //             "serverInstanceId": "c4bf9cba-815e-4b20-a23b-fd404ab6fa15",
    //             "connectedUsers": [
    //                 {
    //                    "UserId": "37bb6e69-167a-4a52-b14d-442d6ba27871",
    //                    "ConnectionId" :"connection 4"
    //                 },
    //                 {
    //                    "UserId": "9fd878fc-f3bf-4bb0-903c-0d81627b8412",
    //                    "ConnectionId" :"connection 5"
    //                 }
    //             ]
    //         }
    //     ]
    // }
    //
    // Expected serialization (new chat room):
    //
    // {
    //     "chatRoomId": "6576b56b-8c96-47d0-b17f-70ef3d84bfda",
    //     "activeConnectionsLimit": 6,
    //     "totalActiveConnections": 0,
    //     "serverInstances": []
    // }
    //
    public class ChatRoomConnectionPool
    {
        public Guid ChatRoomId;
        public int ActiveConnectionsLimit;
        public int TotalActiveConnections;
        public IEnumerable<ServerInstance> ServerInstances = null!;
    }
    //
    // Summary:
    //   Represents all the connected users that are connected to a specific application instance.
    public class ServerInstance
    {
        public Guid ServerInstanceId;
        public IEnumerable<ConnectedUser> ConnectedUsers = null!;
    }
    //
    // Summary:
    //   Represents a connected user with his UUID and his connection id.
    public class ConnectedUser
    {
        public Guid UserId;
        public string ConnectionId = default!;
    }
}