using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Shared.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Backoffice.Config.Database
{
    public class AppDbContext: DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Shared.Models.Task> Tasks { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Role> Roles { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(u => u.Name).IsRequired();
                entity.Property(u => u.Email).IsRequired();
                entity.Property(u => u.Password).IsRequired();
                entity
                    .HasOne(u => u.Role)
                    .WithMany(r => r.Users)
                    .HasForeignKey(u => u.RoleId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(false);
            });

            modelBuilder.Entity<Shared.Models.Task>(entity =>
            {
                entity.Property(u => u.Title).IsRequired();
                entity.Property(u => u.Description).IsRequired(false);
                entity.Property(u => u.StartDate).IsRequired(false);
                entity.Property(u => u.EndDate).IsRequired(false);
                entity.Property(u => u.RealDuration).IsRequired(false);
                entity.Property(u => u.EstimatedDuration).IsRequired(false);

                entity.HasOne(u => u.User)
                    .WithMany(r => r.Tasks)
                    .HasForeignKey(u => u.UserId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(false);

                entity.HasOne(u => u.Project)
                    .WithMany(p => p.Tasks)
                    .HasForeignKey(u => u.ProjectId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(false);
            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.Property(u => u.Title).IsRequired();
                entity.Property(u => u.Description).IsRequired(false);
                entity.Property(u => u.Budget).IsRequired(false);

                entity.HasOne(u => u.Client)
                    .WithMany(r => r.Projects)
                    .HasForeignKey(u => u.ClientId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .IsRequired(false);
            });

            modelBuilder.Entity<Client>(entity =>
            {
                entity.Property(u => u.Name).IsRequired();
                entity.Property(u => u.Address).IsRequired(false);
                entity.Property(u => u.City).IsRequired(false);
                entity.Property(u => u.Country).IsRequired(false);
                entity.Property(u => u.Phone).IsRequired(false);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(u => u.Label).IsRequired();
            });

           // Seed Roles
            modelBuilder.Entity<Role>().HasData(
               new Role { Id = 1, Label = "Administrator" },
               new Role { Id = 2, Label = "Developer" },
               new Role { Id = 3, Label = "Project Manager" }
            );

            // Seed Admin User
            modelBuilder.Entity<User>().HasData(
               new User
               {
                   Id = 1,
                   Name = "Admin",
                   Email = "admin@example.com",
                   Password = "Admin123!", // À remplacer par un mot de passe hashé en production
                   RoleId = 1
               }
            );

            // Seed Sample Project
            modelBuilder.Entity<Project>().HasData(
               new Project
               {
                   Id = 1,
                   Title = "Sample Project",
                   Description = "This is a sample project",
                   CreatedAt = DateTime.UtcNow
               }
            );
        }

        protected void BeforeSaveChanges()
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = DateTime.UtcNow;
                        break;
                }
            }
        }

        public override int SaveChanges()
        {
            BeforeSaveChanges();
            return base.SaveChanges();
        }
    }
}
