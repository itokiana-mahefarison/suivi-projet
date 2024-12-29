using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Shared.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using BCrypt.Net;

namespace Backoffice.Config.Database
{
    public class AppDbContext: DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Tasks> Tasks { get; set; }
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

            modelBuilder.Entity<Tasks>(entity =>
            {
                entity.Property(u => u.Title).IsRequired();
                entity.Property(u => u.Description).IsRequired(false);
                entity.Property(u => u.StartDate).IsRequired(false);
                entity.Property(u => u.EndDate).IsRequired(false);
                entity.Property(u => u.RealDuration).IsRequired(false);
                entity.Property(u => u.EstimatedDuration).IsRequired(false);
                entity.Property(t => t.Status);

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

                entity.HasOne(t => t.ParentTask)
                    .WithMany(t => t.SubTasks)
                    .HasForeignKey(t => t.ParentTaskId)
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
