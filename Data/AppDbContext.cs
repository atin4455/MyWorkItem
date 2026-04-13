using Microsoft.EntityFrameworkCore;
using MyWorkItem.Models;

namespace MyWorkItem.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<WorkItem> WorkItems => Set<WorkItem>();
        public DbSet<UserWorkItemStatus> UserWorkItemStatuses => Set<UserWorkItemStatus>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(x => x.Account)
                .IsUnique();

            modelBuilder.Entity<UserWorkItemStatus>()
                .HasIndex(x => new { x.UserId, x.WorkItemId })
                .IsUnique();

            modelBuilder.Entity<UserWorkItemStatus>()
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserWorkItemStatus>()
                .HasOne(x => x.WorkItem)
                .WithMany()
                .HasForeignKey(x => x.WorkItemId)
                .OnDelete(DeleteBehavior.Cascade);

            var seedTime = new DateTime(2026, 4, 13, 0, 0, 0, DateTimeKind.Utc);
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Account = "admin", Password = "admin123", Role = "Admin", CreatedAt = seedTime },
                new User { Id = 2, Account = "user1", Password = "user123", Role = "User", CreatedAt = seedTime },
                new User { Id = 3, Account = "user2", Password = "user123", Role = "User", CreatedAt = seedTime }
            );

            modelBuilder.Entity<WorkItem>().HasData(
                new WorkItem { Id = 1, Title = "Prepare interview environment", Description = "Check IDE and DB connection.", CreatedAt = seedTime, IsActive = true },
                new WorkItem { Id = 2, Title = "Review system architecture", Description = "Prepare C4 context and container level explanation.", CreatedAt = seedTime.AddMinutes(-10), IsActive = true }
            );
        }
    }
}
