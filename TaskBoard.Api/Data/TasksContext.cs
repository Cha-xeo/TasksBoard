using Microsoft.EntityFrameworkCore;
using TaskBoard.Api.Models;

namespace TaskBoard.Api.Data
{
    public class TasksContext : DbContext
    {
        public TasksContext (DbContextOptions<TasksContext> options)
            : base(options)
        {
        }

        public DbSet<Tasks> Tasks { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<ApiClients> ApiClients { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>().HasQueryFilter(u => u.DeletedAt == null);
            modelBuilder.Entity<Tasks>().HasQueryFilter(t => t.DeletedAt == null);
            modelBuilder.Entity<ApiClients>().HasQueryFilter(a => a.DeletedAt == null);
            modelBuilder.Entity<Tasks>().ToTable(nameof(Models.Tasks));
            modelBuilder.Entity<Users>().ToTable(nameof(Models.Users));
            modelBuilder.Entity<Users>().HasIndex(u => u.UserName).IsUnique();
            modelBuilder.Entity<Users>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<ApiClients>().ToTable(nameof(Models.ApiClients));


        }
    }
}
