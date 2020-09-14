using System;
using System.Linq;
using Dottalk.App.Domain.Models;
using Tests.Dottalk.Support;
using Xunit;

namespace Tests.Dottalk.Unit.Domain
{
    public class ChatRoomLogicTests
    {
        [Fact(DisplayName = "Chat room logic should add a new user to an existing chat room connection pool")]
        public void TestChatRoomLogicShouldAddNewUserToExistingChatRoomConnectionPool()
        {
            // arrange
            var chatRoomId = Guid.NewGuid();
            var instanceId1 = Guid.NewGuid();
            var instanceId2 = Guid.NewGuid();
            var newUserId = Guid.NewGuid();
            var initialConnectionPool = TestingScenarioBuilder
                .BuildChatRoomConnectionPoolTwoInstances(chatRoomId, instanceId1, instanceId2);

            // act: new user that wants to connect to the room being on instance 2
            var updatedConnectionPool = ChatRoomLogic.IncrementChatRoomConnections(newUserId, instanceId2, initialConnectionPool);  // 4 connections

            // assert
            Assert.Equal(5, updatedConnectionPool.TotalActiveConnections);  // 5 connections after
            Assert.Equal(newUserId, updatedConnectionPool.ServerInstances.ElementAt(1).ConnectedUsers.ElementAt(2));  // user has been appended
        }

        [Fact(DisplayName = "Chat room logic should add a new user to a chat room with an empty connection pool")]
        public void TestChatRoomLogicShouldAddNewUserToAChatRoomWithEmptyConnectionPool()
        {
            // arrange
            var chatRoomId = Guid.NewGuid();
            var instanceId = Guid.NewGuid();
            var firstUserId = Guid.NewGuid();
            var emptyConnectionPool = TestingScenarioBuilder.BuildChatRoomConnectionPoolEmpty(chatRoomId);

            // act: FIRST user wants to connect to the room
            var updatedConnectionPool = ChatRoomLogic.IncrementChatRoomConnections(firstUserId, instanceId, emptyConnectionPool);  // 0 connections

            // assert
            Assert.Equal(1, updatedConnectionPool.TotalActiveConnections);  // 1 connection after
            Assert.Equal(firstUserId, updatedConnectionPool.ServerInstances.ElementAt(0).ConnectedUsers.First());  // user has been appended
        }
    }
}