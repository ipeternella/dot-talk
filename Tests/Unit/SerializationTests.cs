using System;
using System.Collections.Generic;
using Dottalk.App.DTOs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Xunit;

namespace Tests.Dottalk.Unit
{
    public class SerializationTests
    {
        public SerializationTests()
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            };
        }

        [Fact(DisplayName = "Should serialize a connection object properly")]
        public void TestShouldSerializerConnectionObject()
        {
            // arrange
            var serverInstance1 = Guid.NewGuid();
            var serverInstance2 = Guid.NewGuid();
            var connectedUserId1 = Guid.NewGuid();
            var connectedUserId2 = Guid.NewGuid();
            var connectedUserId3 = Guid.NewGuid();
            var connectedUserId4 = Guid.NewGuid();

            var serverInstanceUsers1 = new ServerInstanceUsersDTO()
            {
                ServerInstanceId = serverInstance1,
                ConnectedUsers = new List<Guid>() { connectedUserId1, connectedUserId2 }
            };

            var serverInstanceUsers2 = new ServerInstanceUsersDTO()
            {
                ServerInstanceId = serverInstance2,
                ConnectedUsers = new List<Guid>() { connectedUserId3, connectedUserId4 }
            };

            var chatRoomConnections = new ChatRoomConnectionsDTO()
            {
                TotalActiveConnections = 4,
                ServerInstanceUsers = new List<ServerInstanceUsersDTO>() { serverInstanceUsers1, serverInstanceUsers2 }
            };

            // act
            var serializedChatRoomConnectionsStr = JsonConvert.SerializeObject(chatRoomConnections);
            var jsonChatRoomConnection = JObject.Parse(serializedChatRoomConnectionsStr);
            var serverInstances = (JArray)jsonChatRoomConnection["serverInstanceUsers"];

            // assert - guarantees camelCase and not PascalCase on the jsons
            Assert.Equal(4, jsonChatRoomConnection["totalActiveConnections"].Value<int>());
            Assert.Equal(2, serverInstances.Count);

            Assert.Equal(serverInstance1.ToString(),
                jsonChatRoomConnection["serverInstanceUsers"][0]["serverInstanceId"].Value<string>());
            Assert.Equal(serverInstance2.ToString(),
                jsonChatRoomConnection["serverInstanceUsers"][1]["serverInstanceId"].Value<string>());
        }
    }
}