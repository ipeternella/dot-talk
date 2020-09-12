using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dottalk.App.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Dottalk.Infra.Persistence
{
    //
    //  Summary:
    //    The main SQL database context of the application.
    public class DBContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<ChatRoom> ChatRooms { get; set; } = null!;
        public DbSet<ChatMessage> ChatMessages { get; set; } = null!;

        public DBContext(DbContextOptions<DBContext> options) : base(options)
        {

        }
        //
        //  Summary:
        //    Thin wrapper on top of save sync in order to automatically add createdAt and updatedAt fields.
        public override int SaveChanges()
        {
            AutomaticallyAddCreatedAndUpdatedAt();
            return base.SaveChanges();
        }
        //
        //  Summary:
        //    Thin wrapper on top of save async in order to automatically add createdAt and updatedAt fields.
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            AutomaticallyAddCreatedAndUpdatedAt();
            return base.SaveChangesAsync(cancellationToken);
        }
        //
        //  Summary:
        //    Automatically adds the createdAt and updatedAt fields for models that are persisted 
        //    on the database.
        private void AutomaticallyAddCreatedAndUpdatedAt()
        {
            var entitiesOnDbContext = ChangeTracker.Entries<BaseEntity>();

            if (entitiesOnDbContext == null) return; // nothing was changed on DB context

            // createdAt addition
            foreach (var item in entitiesOnDbContext.Where(t => t.State == EntityState.Added))
            {
                item.Entity.CreatedAt = System.DateTime.Now;
                item.Entity.UpdatedAt = System.DateTime.Now;
            }

            // updatedAt addition
            foreach (var item in entitiesOnDbContext.Where(t => t.State == EntityState.Modified))
            {
                item.Entity.UpdatedAt = System.DateTime.Now;
            }
        }
    }
}