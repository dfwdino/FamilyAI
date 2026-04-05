using Microsoft.EntityFrameworkCore;

namespace FamilyAI.Domain.Data
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }

        public DbSet<Models.UserModel> Users { get; set; }
        public DbSet<Models.Role> Roles { get; set; }
        public DbSet<Models.UserPermission> UserPermissions { get; set; }
        public DbSet<Models.ThreadModel> Threads { get; set; }
        public DbSet<Models.ChatLog> ChatLogs { get; set; }
        public DbSet<Models.ParentChild> ParentChildren { get; set; }
        public DbSet<Models.PromptTemplate> PromptTemplates { get; set; }
        public DbSet<Models.UserSetting> UserSettings { get; set; }
        public DbSet<Models.OllamaSetting> OllamaSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Models.UserModel>()
                .HasOne(u => u.UserPermissions)
                .WithOne(up => up.User)
                .HasForeignKey<Models.UserPermission>(up => up.UserId);

            modelBuilder.Entity<Models.ParentChild>()
                .HasOne(pc => pc.Parent)
                .WithMany(u => u.ChildLinks)
                .HasForeignKey(pc => pc.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Models.ParentChild>()
                .HasOne(pc => pc.Child)
                .WithMany(u => u.ParentLinks)
                .HasForeignKey(pc => pc.ChildId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Models.UserSetting>()
                .HasOne(us => us.User)
                .WithMany()
                .HasForeignKey(us => us.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Models.UserSetting>()
                .HasOne(us => us.PromptTemplate)
                .WithMany()
                .HasForeignKey(us => us.PromptTemplateId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
