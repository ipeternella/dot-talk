using System;
using Dottalk.App.Domain.Models;
using Dottalk.App.Ports;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace Dottalk.Infra.Persistence
{
    //
    // Summary:
    //   Repository for managing chat room connections that are sustained through several application instances.
    public class ChatRoomConnectionsRepository : IChatRoomConnectionsRepository
    {
        private readonly ConnectionMultiplexer _redisConnection;
        private readonly IDatabase _redisDb;

        public ChatRoomConnectionsRepository(IConfiguration configuration)
        {
            var redisConnectionString = configuration.GetValue<string>("Redis:ConnectionString");
            var redisDatabaseNumber = configuration.GetValue<int>("Redis:ConnectionRepositoryDatabaseNumber");

            if (redisConnectionString == null || redisDatabaseNumber == 0)
                throw new Exception("Redis connection is misconfigured!");

            _redisConnection = ConnectionMultiplexer.Connect(redisConnectionString);
            _redisDb = _redisConnection.GetDatabase(redisDatabaseNumber);
        }

        public void AddChatRoomConnection(Guid chatRoomId, Guid userId, Guid ServerInstanceId)
        {
            // now we know how to serialize DTOs...
            throw new NotImplementedException();
        }

        public void RemoveChatRoomConnection(Guid chatRoomId, Guid userId, Guid ServerInstanceId)
        {
            throw new NotImplementedException();
        }

        public int TotalActiveConnections(Guid chatRoomId)
        {
            throw new NotImplementedException();
        }

        public ChatRoomConnections GetChatRoomConnections(Guid chatRoomId)
        {
            throw new NotImplementedException();
        }
    }
}