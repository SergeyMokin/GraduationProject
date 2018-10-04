using GraduationProjectModels;
using Microsoft.EntityFrameworkCore;

namespace GraduationProjectRepositories
{
    // Realization of DbContext to work with database.
    public sealed class GraduationProjectContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Password> Passwords { get; set; }
        public DbSet<BlankFile> BlankFiles { get; set; }
        public DbSet<QuestionEntity> Questions { get; set; }
        public DbSet<BlankType> BlankTypes { get; set; }

        public GraduationProjectContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }

        // Configure models using Fluent API.
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>(entity => 
            {
                entity.HasIndex(x => x.Email).IsUnique();
            });

            builder.Entity<BlankFileUser>()
                .HasKey(x => new { x.BlankFileId, x.UserId });

            builder.Entity<BlankFileUser>()
                .HasOne(x => x.User)
                .WithMany(x => x.BlankFileUsers)
                .HasForeignKey(x => x.UserId);

            builder.Entity<BlankFileUser>()
                .HasOne(x => x.BlankFile)
                .WithMany(x => x.BlankFileUsers)
                .HasForeignKey(x => x.BlankFileId);
        }
    }
}
