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

        // Content flagging
        public DbSet<Models.GlobalFlagRule> GlobalFlagRules { get; set; }
        public DbSet<Models.ParentFlagRule> ParentFlagRules { get; set; }
        public DbSet<Models.ParentScanSetting> ParentScanSettings { get; set; }
        public DbSet<Models.ThreadScanResult> ThreadScanResults { get; set; }
        public DbSet<Models.ThreadFlagDetail> ThreadFlagDetails { get; set; }

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

            // ParentFlagRule → User (restrict to avoid cascade conflicts)
            modelBuilder.Entity<Models.ParentFlagRule>()
                .HasOne(r => r.Parent)
                .WithMany()
                .HasForeignKey(r => r.ParentUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Models.ParentFlagRule>()
                .HasOne(r => r.GlobalRule)
                .WithMany()
                .HasForeignKey(r => r.GlobalRuleId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Models.ParentScanSetting>()
                .HasOne(s => s.Parent)
                .WithMany()
                .HasForeignKey(s => s.ParentUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Models.ThreadScanResult>()
                .HasOne(r => r.Thread)
                .WithMany()
                .HasForeignKey(r => r.ThreadId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Models.ThreadScanResult>()
                .HasOne(r => r.ScannedByParent)
                .WithMany()
                .HasForeignKey(r => r.ScannedByParentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Models.ThreadFlagDetail>()
                .HasOne(d => d.ScanResult)
                .WithMany(r => r.FlagDetails)
                .HasForeignKey(d => d.ThreadScanResultId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Models.ThreadFlagDetail>()
                .HasOne(d => d.Message)
                .WithMany()
                .HasForeignKey(d => d.MessageId)
                .OnDelete(DeleteBehavior.SetNull);

            // Seed global flag rules
            modelBuilder.Entity<Models.GlobalFlagRule>().HasData(
                // ── Keywords ──────────────────────────────────────────────────────
                new Models.GlobalFlagRule { Id = 1, IsDeleted = false, Type = Models.FlagRuleType.Keyword, Value = "suicide", Description = "Direct reference to suicide" },
                new Models.GlobalFlagRule { Id = 2, IsDeleted = false, Type = Models.FlagRuleType.Keyword, Value = "kill myself", Description = "Self-harm phrase" },
                new Models.GlobalFlagRule { Id = 3, IsDeleted = false, Type = Models.FlagRuleType.Keyword, Value = "self harm", Description = "Self-harm phrase" },
                new Models.GlobalFlagRule { Id = 4, IsDeleted = false, Type = Models.FlagRuleType.Keyword, Value = "cutting myself", Description = "Self-harm phrase" },
                new Models.GlobalFlagRule { Id = 5, IsDeleted = false, Type = Models.FlagRuleType.Keyword, Value = "drugs", Description = "General drug reference" },
                new Models.GlobalFlagRule { Id = 6, IsDeleted = false, Type = Models.FlagRuleType.Keyword, Value = "marijuana", Description = "Cannabis reference" },
                new Models.GlobalFlagRule { Id = 7, IsDeleted = false, Type = Models.FlagRuleType.Keyword, Value = "weed", Description = "Cannabis slang" },
                new Models.GlobalFlagRule { Id = 8, IsDeleted = false, Type = Models.FlagRuleType.Keyword, Value = "cocaine", Description = "Hard drug reference" },
                new Models.GlobalFlagRule { Id = 9, IsDeleted = false, Type = Models.FlagRuleType.Keyword, Value = "heroin", Description = "Hard drug reference" },
                new Models.GlobalFlagRule { Id = 10, IsDeleted = false, Type = Models.FlagRuleType.Keyword, Value = "meth", Description = "Hard drug reference" },
                new Models.GlobalFlagRule { Id = 11, IsDeleted = false, Type = Models.FlagRuleType.Keyword, Value = "fentanyl", Description = "Hard drug reference" },
                new Models.GlobalFlagRule { Id = 12, IsDeleted = false, Type = Models.FlagRuleType.Keyword, Value = "alcohol", Description = "Alcohol reference" },
                new Models.GlobalFlagRule { Id = 13, IsDeleted = false, Type = Models.FlagRuleType.Keyword, Value = "my address", Description = "Sharing personal location information" },
                new Models.GlobalFlagRule { Id = 14, IsDeleted = false, Type = Models.FlagRuleType.Keyword, Value = "my phone number", Description = "Sharing personal contact information" },
                new Models.GlobalFlagRule { Id = 15, IsDeleted = false, Type = Models.FlagRuleType.Keyword, Value = "my school", Description = "Sharing school information with strangers" },

                // ── Topics ────────────────────────────────────────────────────────
                new Models.GlobalFlagRule { Id = 16, IsDeleted = false, Type = Models.FlagRuleType.Topic, Value = "Self-harm or suicide", Description = "Any discussion of self-harm, suicidal thoughts, or hurting oneself" },
                new Models.GlobalFlagRule { Id = 17, IsDeleted = false, Type = Models.FlagRuleType.Topic, Value = "Drug or alcohol use", Description = "Discussion of using, obtaining, or glorifying drugs or alcohol" },
                new Models.GlobalFlagRule { Id = 18, IsDeleted = false, Type = Models.FlagRuleType.Topic, Value = "Violence or physical threats", Description = "Threats of violence, planning to hurt someone, or glorifying violence" },
                new Models.GlobalFlagRule { Id = 19, IsDeleted = false, Type = Models.FlagRuleType.Topic, Value = "Sexual or adult content", Description = "Sexually explicit topics, pornography, or adult themes inappropriate for children" },
                new Models.GlobalFlagRule { Id = 20, IsDeleted = false, Type = Models.FlagRuleType.Topic, Value = "Bullying or harassment", Description = "Bullying others, being bullied, or discussions of targeted harassment" },
                new Models.GlobalFlagRule { Id = 21, IsDeleted = false, Type = Models.FlagRuleType.Topic, Value = "Sharing personal information", Description = "Sharing home address, phone number, school name, or other identifying information" },
                new Models.GlobalFlagRule { Id = 22, IsDeleted = false, Type = Models.FlagRuleType.Topic, Value = "Online predator or grooming behavior", Description = "Adult asking child for personal info, photos, or to meet in person" },
                new Models.GlobalFlagRule { Id = 23, IsDeleted = false, Type = Models.FlagRuleType.Topic, Value = "Weapons", Description = "Discussion of obtaining or using weapons" },

                // ── Tones ─────────────────────────────────────────────────────────
                new Models.GlobalFlagRule { Id = 24, IsDeleted = false, Type = Models.FlagRuleType.Tone, Value = "Threatening or aggressive", Description = "Messages with a threatening, hostile, or intimidating tone directed at a person" },
                new Models.GlobalFlagRule { Id = 25, IsDeleted = false, Type = Models.FlagRuleType.Tone, Value = "Sexually suggestive", Description = "Messages with a sexually suggestive or inappropriate tone for a child" },
                new Models.GlobalFlagRule { Id = 26, IsDeleted = false, Type = Models.FlagRuleType.Tone, Value = "Deeply distressed or hopeless", Description = "Messages expressing extreme despair, hopelessness, or emotional crisis" }
            );
        }
    }
}
