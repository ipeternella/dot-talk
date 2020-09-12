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
    public class WebServerBaseTestCase<TStartup> : IDisposable where TStartup : class
    {
        private readonly IDbContextTransaction _transaction;
        protected readonly HttpClient Client;  // subclasses can use the HTTP client for endpoint testing
        protected DBContext DB { get; }

        protected WebServerBaseTestCase()
        {
            // TStartup can be any class that configures DI and inject mocked services
            var builder = WebHost.CreateDefaultBuilder()
                .UseStartup<TStartup>()
                .UseSerilog();
            var server = new TestServer(builder);
            var services = server.Host.Services;

            Client = server.CreateClient();
            DB = services.GetRequiredService<DBContext>();

            // Transaction used on every test
            _transaction = DB.Database.BeginTransaction();
        }

        public void Dispose()
        {
            if (_transaction == null) return;

            _transaction.Rollback();
            _transaction.Dispose();
        }
    }
}