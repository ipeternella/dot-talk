using System.Collections.Generic;
using System.Linq;
using Dottalk.App.Domain.Models;
using Dottalk.Controllers;
using Dottalk.Infra.Persistence;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;

namespace Dottalk
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor()
                .AddDbContext<DBContext>(options => options.UseNpgsql(Configuration.GetValue<string>("Databases:Postgres:ConnectionString")))
                .AddSingleton(options => ActivatorUtilities.CreateInstance<RedisContext>(options, Configuration))
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

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            if (!env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHttpsRedirection();
            }
            app.UseSerilogRequestLogging();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChatHubController>("/chathub");
            });

            Migrate(app, logger, executeSeedDb: env.IsDevelopment());
        }

        public static void Migrate(IApplicationBuilder app, ILogger<Startup> logger, bool executeSeedDb = false)
        {
            using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            using var context = serviceScope.ServiceProvider.GetService<DBContext>();

            // always execute possible missing migrations
            if (context.Database.GetPendingMigrations().ToList().Any())
            {
                logger.LogInformation("Applying migrations...");
                context.Database.Migrate();
            }

            // seeding DB only when asked
            if (!executeSeedDb) return;

            logger.LogInformation("Seeding the database...");
            SeedDb(context, logger);
        }

        private static void SeedDb(DBContext context, ILogger<Startup> logger)
        {
            if (context.ChatRooms.Any())
            {
                logger.LogInformation("Database has already been seeded. Skipping it...");
                return;
            }

            logger.LogInformation("Saving entities...");
            var chatRooms = new List<ChatRoom>
            {
                new ChatRoom {Name = "Chat Room 1"},
                new ChatRoom {Name = "Chat Room 2"},
                new ChatRoom {Name = "Chat Room 3"}
            };

            context.AddRange(chatRooms);
            context.SaveChanges();

            logger.LogInformation("Database has been seeded successfully.");
        }
    }
}
