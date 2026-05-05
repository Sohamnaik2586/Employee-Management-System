using Microsoft.EntityFrameworkCore;
using EmployeeManagement.Domain.Entities;

namespace EmployeeManagement.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>()
                .Property(e => e.Salary)
                .HasPrecision(10, 2); // total digits = 18, decimal places = 2
            modelBuilder.Entity<Employee>()
                .Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            modelBuilder.Entity<Employee>()
                .Property(e => e.Department)
                .HasMaxLength(25)
                .IsUnicode(false);

        }

    }
}