using Microsoft.EntityFrameworkCore;
using app.Models;

namespace app.Data
{
    /// <summary>
    /// Contexto de base de datos para el sistema de gestión de empleados
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSets para todas las entidades
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<DepartmentEmployee> DepartmentEmployees { get; set; }
        public DbSet<DepartmentManager> DepartmentManagers { get; set; }
        public DbSet<Title> Titles { get; set; }
        public DbSet<Salary> Salaries { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de la entidad Employee
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.EmpNo);
                entity.Property(e => e.CI).IsRequired().HasMaxLength(50);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.CI).IsUnique();
            });

            // Configuración de la entidad Department
            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(e => e.DeptNo);
                entity.Property(e => e.DeptName).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.DeptName).IsUnique();
            });

            // Configuración de la entidad DepartmentEmployee
            modelBuilder.Entity<DepartmentEmployee>(entity =>
            {
                entity.HasKey(e => new { e.EmpNo, e.DeptNo });
                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.DepartmentEmployees)
                    .HasForeignKey(d => d.EmpNo)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(d => d.Department)
                    .WithMany(p => p.DepartmentEmployees)
                    .HasForeignKey(d => d.DeptNo)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de la entidad DepartmentManager
            modelBuilder.Entity<DepartmentManager>(entity =>
            {
                entity.HasKey(e => new { e.EmpNo, e.DeptNo });
                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.DepartmentManagers)
                    .HasForeignKey(d => d.EmpNo)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(d => d.Department)
                    .WithMany(p => p.DepartmentManagers)
                    .HasForeignKey(d => d.DeptNo)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de la entidad Title
            modelBuilder.Entity<Title>(entity =>
            {
                entity.HasKey(e => new { e.EmpNo, e.TitleName, e.FromDate });
                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.Titles)
                    .HasForeignKey(d => d.EmpNo)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de la entidad Salary
            modelBuilder.Entity<Salary>(entity =>
            {
                entity.HasKey(e => new { e.EmpNo, e.FromDate });
                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.Salaries)
                    .HasForeignKey(d => d.EmpNo)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de la entidad User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.EmpNo);
                entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasOne(d => d.Employee)
                    .WithOne(p => p.User)
                    .HasForeignKey<User>(d => d.EmpNo)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de la entidad AuditLog
            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Usuario).IsRequired().HasMaxLength(50);
                entity.Property(e => e.DetalleCambio).IsRequired().HasMaxLength(250);
                entity.HasOne(d => d.Employee)
                    .WithMany(p => p.AuditLogs)
                    .HasForeignKey(d => d.EmpNo)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
