using System;
using System.IO;
using System.Net.Http;
using AutoMapper;
using Dottalk;
using Dottalk.App.Ports;
using Dottalk.App.Services;
using Dottalk.Infra.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;
using Tests.Dottalk.Support;

namespace Tests.Hangman.Support
{
    //
    //  Summary:
    //    Base test case to be inherited when a testing web server is not used or is just too much for the tests.
    //    This class can handle transactions/rollbacks automatically for restoring the database state.
    public class BaseTestCase : IDisposable
    {
        private readonly IDbContextTransaction _transaction;
        protected readonly HttpClient Client;  // subclasses can use
        protected DBContext DB { get; }
        protected IChatRoomService ChatRoomService { get; }
        protected TestingScenarioBuilder TestingScenarioBuilder { get; set; }

        public BaseTestCase()
        {
            // builds configuration based on jsons and env variables
            var env = Environment.GetEnvironmentVariable("DOTNETCORE_ENVIRONMENT");
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            if (env != null) configuration.AddJsonFile($"appsettings.{env}.json", optional: true);  // local development: stdout
            var builtConfig = configuration.AddEnvironmentVariables().Build();

            // builds DI container by setting up all the services
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection, builtConfig);

            // builds service provider from the DI container to get services
            var serviceProvider = serviceCollection.BuildServiceProvider();

            // Services to be used by the tests
            DB = serviceProvider.GetRequiredService<DBContext>();
            ChatRoomService = serviceProvider.GetRequiredService<IChatRoomService>();
            TestingScenarioBuilder = new TestingScenarioBuilder();

            // Transactions used on every test -- add DB, Mongo, etc.
            _transaction = DB.Database.BeginTransaction();
        }

        public void ConfigureServices(IServiceCollection services, IConfigurationRoot configuration)
        {
            // this is required to add the controllers of the main project
            var startupAssembly = typeof(Startup).Assembly;

            services.AddHttpContextAccessor()
                .AddDbContext<DBContext>(options => options.UseNpgsql(configuration.GetValue<string>("Databases:Postgres:ConnectionString")), ServiceLifetime.Singleton)
                .AddSingleton(options => ActivatorUtilities.CreateInstance<RedisContext>(options, configuration))
                .AddScoped<IChatRoomService, ChatRoomService>()
                .AddScoped<IUserService, UserService>()
                .AddLogging(builder => builder.AddSerilog(dispose: true))
                .AddAutoMapper(typeof(Startup))
                .AddControllers();

            services.AddSignalR();

            // global json serialization settings
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            };
        }

        public void Dispose()
        {
            if (_transaction == null) return;

            _transaction.Rollback();
            _transaction.Dispose();
        }
    }
}