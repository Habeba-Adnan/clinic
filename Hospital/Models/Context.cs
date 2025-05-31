using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace Hospital.Models
{
    public class Context:DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options) { }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<Patient> Patients { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Patient>()
               .Property(p => p.Gender)
               .HasConversion<string>();

            //modelBuilder.Entity<Patient>().HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
