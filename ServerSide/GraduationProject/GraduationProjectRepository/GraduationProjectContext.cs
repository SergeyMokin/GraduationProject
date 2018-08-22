using GraduationProjectModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GraduationProjectRepository
{
    public class GraduationProjectContext : DbContext
    {
        public GraduationProjectContext()
        { }

        public GraduationProjectContext(DbContextOptions<GraduationProjectContext> options) : base(options)
        { }

        public DbSet<User> Users { get; set; }
        public DbSet<Password> Passwords { get; set; }
        public DbSet<BlankFile> BlankFiles { get; set; }
    }
}
