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
            var connectionId = "fakeId";
            var chatRoomId = Guid.NewGuid();
            var instanceId1 = Guid.NewGuid();
            var instanceId2 = Guid.NewGuid();
            var newUserId = Guid.NewGuid();
            var initialConnectionPool = TestingScenarioBuilder
                .BuildChatRoomConnectionPoolTwoInstances(chatRoomId, 6, instanceId1, instanceId2);

            // act: new user that wants to connect to the room being on instance 2
            var updatedConnectionPool = ChatRoomLogic.IncrementChatRoomConnectionPool(newUserId, instanceId2, connectionId, initialConnectionPool);  // 4 connections

            // assert
            Assert.Equal(5, updatedConnectionPool.TotalActiveConnections);  // 5 connections after
            Assert.Equal(newUserId, updatedConnectionPool.ServerInstances.ElementAt(1).ConnectedUsers.ElementAt(2).UserId);  // user has been appended
            Assert.Equal(connectionId, updatedConnectionPool.ServerInstances.ElementAt(1).ConnectedUsers.ElementAt(2).ConnectionId);  // user has been appended
        }

        [Fact(DisplayName = "Chat room logic should remove a user from an existing chat room connection pool")]
        public void TestChatRoomLogicShouldRemoveUserFromAnExistingChatRoomConnectionPool()
        {
            // arrange
            var chatRoomId = Guid.NewGuid();
            var instanceId1 = Guid.NewGuid();
            var instanceId2 = Guid.NewGuid();
            var initialConnectionPool = TestingScenarioBuilder
                .BuildChatRoomConnectionPoolTwoInstances(chatRoomId, 6, instanceId1, instanceId2);

            // act: remove user with connection 1 from room  -- starts with 4 users
            var updatedConnectionPool = ChatRoomLogic.DecrementChatRoomConnectionPool(instanceId1, "connection 1", initialConnectionPool);

            // assert
            var remainingConnectedUser = updatedConnectionPool.ServerInstances.First().ConnectedUsers.First();

            Assert.Equal(3, updatedConnectionPool.TotalActiveConnections);  // 3 users after one is removed
            Assert.Single(updatedConnectionPool.ServerInstances.First().ConnectedUsers);  // only 1 connection on instanceId1
            Assert.Equal("connection 2", remainingConnectedUser.ConnectionId);

        }

        [Fact(DisplayName = "Chat room logic should add a new user to a chat room with an empty connection pool")]
        public void TestChatRoomLogicShouldAddNewUserToAChatRoomWithEmptyConnectionPool()
        {
            // arrange
            var connectionId = "fakeId";
            var chatRoomId = Guid.NewGuid();
            var instanceId = Guid.NewGuid();
            var firstUserId = Guid.NewGuid();
            var emptyConnectionPool = TestingScenarioBuilder.BuildChatRoomConnectionPoolEmpty(chatRoomId);

            // act: FIRST user wants to connect to the room
            var updatedConnectionPool = ChatRoomLogic.IncrementChatRoomConnectionPool(firstUserId, instanceId, connectionId, emptyConnectionPool);  // 0 connections

            // assert
            Assert.Equal(1, updatedConnectionPool.TotalActiveConnections);  // 1 connection after
            Assert.Equal(firstUserId, updatedConnectionPool.ServerInstances.ElementAt(0).ConnectedUsers.First().UserId);  // user has been appended
            Assert.Equal(connectionId, updatedConnectionPool.ServerInstances.ElementAt(0).ConnectedUsers.First().ConnectionId);  // user has been appended
        }
    }
}