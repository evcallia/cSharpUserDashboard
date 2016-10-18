using Microsoft.EntityFrameworkCore;

namespace userDashboard.Models
{
    public class UserDashboardContext : DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder){
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Message>()
                .HasOne(p => p.poster)
                .WithMany(m => m.messagesFrom);

            modelBuilder.Entity<User>()
                .HasMany(m => m.messagesTo)
                .WithOne(m => m.user);

        }

        public UserDashboardContext(DbContextOptions<UserDashboardContext> options) : base(options)
        { }
        public DbSet<User> users { get; set; }

        public DbSet<Message> messages { get; set; }

        public DbSet<Comment> comments { get; set; }
    }
}