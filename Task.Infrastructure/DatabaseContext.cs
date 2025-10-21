using Microsoft.EntityFrameworkCore;
using Task.Domain.Models;

namespace Task.Infrastructure
{
    public class DatabaseContext : DbContext
    {
        public DbSet<MyTask> Tasks { get; set; }
        public DbSet<User> Users { get; set; }

        public DatabaseContext(DbContextOptions options) : base(options)
        {

        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Define one-to-many relationship

            modelBuilder.Entity<MyTask>()
                .HasOne(t => t.User)
                .WithMany(u => u.Tasks)
                .HasForeignKey(t => t.CreatedBy)
                .OnDelete(DeleteBehavior.Cascade);
        }

    }
}
