using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Dottalk.Infra.Persistence
{
    //
    // Summary:
    //   Redis context for manipulating Redis sessions.
    public class RedisContext
    {
        public ConnectionMultiplexer RedisConnection;
        public IDatabase DB;

        public RedisContext(IConfiguration configuration)
        {
            var redisConnectionString = configuration.GetValue<string>("Redis:ConnectionString");
            var redisDatabaseNumber = configuration.GetValue<int>("Redis:ConnectionRepositoryDatabaseNumber");

            if (redisConnectionString == null || redisDatabaseNumber == 0)
                throw new Exception("Redis connection is misconfigured!");

            RedisConnection = ConnectionMultiplexer.Connect(redisConnectionString);
            DB = RedisConnection.GetDatabase(redisDatabaseNumber);
        }
        //
        // Summary:
        //   Gets a key from Redis and deserializes it to a specific type. If key is not found,
        //   returns the default value of the generid type T.
        public async Task<T?> GetKey<T>(string keyName) where T : class
        {
            var redisValue = await DB.StringGetAsync(keyName);
            if (redisValue.IsNull) return default;

            return JsonConvert.DeserializeObject<T>(redisValue);
        }
        //
        // Summary:
        //   Gets a key from Redis and deserializes it to a specific type based on a Guid key. If key is not found,
        //   returns the default value of the generid type T.
        public async Task<T?> GetKey<T>(Guid keyName) where T : class
        {
            var redisValue = await DB.StringGetAsync(keyName.ToString());
            if (redisValue.IsNull) return default;

            return JsonConvert.DeserializeObject<T>(redisValue);
        }
        //
        // Summary:
        //   Sets a key from Redis by serializing it to a specific type.
        public async Task SetKey<T>(string keyName, T keyValue, TimeSpan? timeSpan)
        {
            var redisValue = JsonConvert.SerializeObject(keyValue);
            await DB.StringSetAsync(keyName, redisValue, expiry: timeSpan);
        }
        //
        // Summary:
        //   Sets a key from Redis by serializing it to a specific type based on a Guid key.
        public async Task SetKey<T>(Guid keyName, T keyValue, TimeSpan? timeSpan)
        {
            var redisValue = JsonConvert.SerializeObject(keyValue);
            await DB.StringSetAsync(keyName.ToString(), redisValue, expiry: timeSpan);
        }
    }
}