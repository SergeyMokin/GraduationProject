using GraduationProjectModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraduationProjectRepositories
{
    // Realization of DbContext to work with database.
    public class GraduationProjectContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Password> Passwords { get; set; }
        public DbSet<BlankFile> BlankFiles { get; set; }

        public GraduationProjectContext(DbContextOptions<GraduationProjectContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        // Configure models using Fluent API.
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>(entity => {
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
