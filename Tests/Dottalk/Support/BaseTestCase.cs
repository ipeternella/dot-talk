using System;
using System.Net.Http;
using Dottalk.Infra.Persistence;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Tests.Hangman.Support
{
    //
    //  Summary:
    //    Base test case to be inherited when a full end-to-end testing is desired, e.g., full integration tests
    //    of the web host endpoints, or tests that require full dependency injection setup according to the
    //    TStartup configuration. This class can handle transactions/rollbacks automatically for restoring the database state.
    //
    //    Many start up classes can be used so that different mocks and DI setup can be configured and use by the tests.
    public class BaseTestCase<TStartup> : IDisposable where TStartup : class
    {
        // private state managed by the base test case
        private readonly IDbContextTransaction _transaction;

        // variables for subclasses
        protected IServiceProvider ServiceProvider;  // Services for calling on tests
        protected readonly HttpClient Client;  // HTTP client for full endpoint testing
        protected DBContext DB { get; }  // DB for reaching the database for asserts
        protected RedisContext Redis { get; }

        public BaseTestCase()
        {
            // TStartup is used to configure DI with services and mocked services on the ServiceCollection
            var builder = WebHost.CreateDefaultBuilder()
                .UseStartup<TStartup>()
                .UseSerilog();
            var server = new TestServer(builder);
            var serviceProvider = server.Host.Services;

            // Utility variables used by the tests
            Client = server.CreateClient();
            ServiceProvider = serviceProvider;  // service provider for test classes to retrieve services from ServiceCollection
            DB = serviceProvider.GetRequiredService<DBContext>();
            Redis = serviceProvider.GetRequiredService<RedisContext>();

            // Transaction used on every test  -- add other transactions if desired: Mongo, etc.
            _transaction = DB.Database.BeginTransaction();
        }
        //
        //  Summary:
        //    Tear down and state-restore to be performed after every test
        public void Dispose()
        {
            if (_transaction == null) return;

            _transaction.Rollback();
            _transaction.Dispose();
        }
    }
}