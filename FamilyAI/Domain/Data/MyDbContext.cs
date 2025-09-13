using Microsoft.EntityFrameworkCore;

namespace FamilyAI.Domain.Data
{
    public class MyDbContext : DbContext
    {

        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {

        }

        public DbSet<Models.UserModel> Users { get; set; }
        public DbSet<Models.ThreadModel> Threads { get; set; }
        public DbSet<Models.UserPermission> UserPermissions { get; set; }
        public DbSet<Models.ChatLog> ChatLogs { get; set; }

    }
}
