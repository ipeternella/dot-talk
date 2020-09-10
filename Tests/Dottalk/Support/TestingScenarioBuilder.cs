using System;
using System.Collections.Generic;
using Dottalk.App.Domain.Models;

namespace Tests.Dottalk.Support
{
    public class TestingScenarioBuilder
    {
        public TestingScenarioBuilder()
        {

        }

        public static ChatRoomConnections BuildChatRoomConnectionsWithFourUsers()
        {
            var chatRoomId = Guid.NewGuid();
            var serverInstanceId1 = Guid.NewGuid();
            var serverInstanceId2 = Guid.NewGuid();
            var connectedUserId1 = Guid.NewGuid();
            var connectedUserId2 = Guid.NewGuid();
            var connectedUserId3 = Guid.NewGuid();
            var connectedUserId4 = Guid.NewGuid();

            var serverInstance1 = new ServerInstance()
            {
                ServerInstanceId = serverInstanceId1,
                ConnectedUsers = new List<Guid>() { connectedUserId1, connectedUserId2 }
            };

            var serverInstance2 = new ServerInstance()
            {
                ServerInstanceId = serverInstanceId2,
                ConnectedUsers = new List<Guid>() { connectedUserId3, connectedUserId4 }
            };

            var chatRoomConnections = new ChatRoomConnections()
            {
                ChatRoomId = chatRoomId,
                TotalActiveConnections = 4,
                ActiveConnectionsLimit = 6,
                ServerInstances = new List<ServerInstance>() { serverInstance1, serverInstance2 }
            };

            return chatRoomConnections;
        }
    }
}