using System.Linq;
using Dottalk;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AutoMapper;
using Serilog;
using Dottalk.Infra.Persistence;
using Dottalk.App.Ports;
using Dottalk.App.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Tests.Hangman.Support
{
    //
    //  Summary:
    //   
    public class TestingStartUp
    {
        public TestingStartUp(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // this is required to add the controllers of the main Hangman project
            var startupAssembly = typeof(Startup).Assembly;

            services.AddHttpContextAccessor()
                .AddDbContext<DBContext>(options => options.UseNpgsql(Configuration.GetValue<string>("Databases:Postgres:ConnectionString")))
                .AddSingleton(options => ActivatorUtilities.CreateInstance<RedisContext>(options, Configuration))
                .AddScoped<IChatRoomService, ChatRoomService>()
                .AddScoped<IUserService, UserService>()
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

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment() || env.IsEnvironment("Local"))
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            // middleware for condensing many access log lines into a SINGLE useful one
            app.UseSerilogRequestLogging();

            app.UseRouting();

            app.UseAuthorization();

            // middleware for activating the health check UI
            // app.UseHealthChecksUI(options => options.UIPath = "/healthcheck-dashboard");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            Migrate();
        }

        private void Migrate()
        {
            // testing migrations
            var dbConnectionString = Configuration.GetConnectionString("DBConnection");
            var options = new DbContextOptionsBuilder<DBContext>()
                .UseNpgsql(dbConnectionString)
                .Options;

            var context = new DBContext(options);

            // always execute possible missing migrations
            if (!context.Database.GetPendingMigrations().ToList().Any()) return;
            context.Database.Migrate();
        }
    }
}