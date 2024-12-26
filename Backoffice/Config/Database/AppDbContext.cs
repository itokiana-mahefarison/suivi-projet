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

            modelBuilder.Entity<User>()
                .Property(u => u.Name)
                .IsRequired(false);

            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.Password)
                .IsRequired();

            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .IsRequired(false);

            modelBuilder.Entity<Shared.Models.Task>()
                .Property(t => t.Title)
                .IsRequired();

            modelBuilder.Entity<Shared.Models.Task>()
                .Property(t => t.Description)
                .IsRequired(false);

            modelBuilder.Entity<Shared.Models.Task>()
                .Property(t => t.StartDate)
                .IsRequired(false);

            modelBuilder.Entity<Shared.Models.Task>()
                .Property(t => t.EndDate)
                .IsRequired(false);

            modelBuilder.Entity<Shared.Models.Task>()
                .Property(t => t.RealDuration)
                .IsRequired(false);

            modelBuilder.Entity<Shared.Models.Task>()
                .Property(t => t.EstimatedDuration)
                .IsRequired(false);

            modelBuilder.Entity<Shared.Models.Task>()
                .HasOne(t => t.User)
                .WithMany(r => r.Tasks)
                .IsRequired(false);

            modelBuilder.Entity<Shared.Models.Task>()
                .HasOne(t => t.Project)
                .WithMany(p => p.Tasks)
                .IsRequired(false);

            modelBuilder.Entity<Project>()
                .Property(p => p.Title)
                .IsRequired();

            modelBuilder.Entity<Project>()
                .Property(p => p.Description)
                .IsRequired(false);

            modelBuilder.Entity<Project>()
                .Property(p => p.Budget)
                .IsRequired(false);

            modelBuilder.Entity<Project>()
                .HasOne(p => p.Client)
                .WithMany(c => c.Projects)
                .IsRequired(false);

            modelBuilder.Entity<Client>()
                .Property(c => c.Name)
                .IsRequired();

            modelBuilder.Entity<Client>()
                .Property(c => c.Address)
                .IsRequired(false);

            modelBuilder.Entity<Client>()
                .Property(c => c.City)
                .IsRequired(false);

            modelBuilder.Entity<Client>()
                .Property(c => c.Country)
                .IsRequired(false);

            modelBuilder.Entity<Client>()
                .Property(c => c.Phone)
                .IsRequired(false);

            modelBuilder.Entity<Role>()
                .Property(r => r.Label)
                .IsRequired();
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
