using Microsoft.EntityFrameworkCore;

namespace UsersTasks.Models
{
    public class UsersTasksContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<UserTask> UsersTasks { get; set; }

        public UsersTasksContext(DbContextOptions<UsersTasksContext> options) : base(options){}
    }
}
