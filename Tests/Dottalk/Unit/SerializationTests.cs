using System;
using System.Collections.Generic;
using Dottalk.App.Domain.Models;
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
                ServerInstances = new List<ServerInstance>() { serverInstance1, serverInstance2 }
            };

            // act -- serializes a complex object using the app settings
            var serializedChatRoomConnectionsStr = JsonConvert.SerializeObject(chatRoomConnections);

            // assert - guarantees camelCase and not PascalCase on the jsons
            var jsonChatRoomConnection = JObject.Parse(serializedChatRoomConnectionsStr);
            var serverInstances = (JArray)jsonChatRoomConnection["serverInstances"];

            Assert.Equal(chatRoomId.ToString(), jsonChatRoomConnection["chatRoomId"]);
            Assert.Equal(4, jsonChatRoomConnection["totalActiveConnections"].Value<int>());
            Assert.Equal(2, serverInstances.Count);

            Assert.Equal(serverInstanceId1.ToString(),
                jsonChatRoomConnection["serverInstances"][0]["serverInstanceId"].Value<string>());
            Assert.Equal(serverInstanceId2.ToString(),
                jsonChatRoomConnection["serverInstances"][1]["serverInstanceId"].Value<string>());
        }
    }
}