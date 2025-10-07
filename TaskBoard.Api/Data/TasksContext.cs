using Microsoft.EntityFrameworkCore;

namespace TaskBoard.Api.Data
{
    public class TasksContext : DbContext
    {
        public TasksContext (DbContextOptions<TasksContext> options)
            : base(options)
        {
        }

        public DbSet<Models.Tasks> Tasks { get; set; }
        public DbSet<Models.Users> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Models.Tasks>().ToTable(nameof(Models.Tasks));
            modelBuilder.Entity<Models.Users>().ToTable(nameof(Models.Users));
        }
    }
}
