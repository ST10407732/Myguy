using Microsoft.EntityFrameworkCore;
using MYGUYY.Models;

namespace MYGUYY.Data
{
    public class MYGUYYContext : DbContext
    {
        public MYGUYYContext(DbContextOptions<MYGUYYContext> options) : base(options) { }

        // DbSets for the models used in AccountController
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<TaskRequest> TaskRequests { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Stop> Stops { get; set; }
        //public DbSet<DriverProfile> DriverProfiles { get; set; }
        public DbSet<TransactionModel> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure User entity
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id); // Primary Key

            modelBuilder.Entity<User>()
                .Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<User>()
                .Property(u => u.PasswordHash)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .IsRequired();

            // Configure TaskRequest entity
            modelBuilder.Entity<TaskRequest>()
                .HasKey(tr => tr.Id);

            modelBuilder.Entity<TaskRequest>()
                .Property(tr => tr.Status)
                .IsRequired()
                .HasMaxLength(20);

            modelBuilder.Entity<TaskRequest>()
                .Property(tr => tr.CreatedAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<TaskRequest>()
                .HasOne(tr => tr.Client)
                .WithMany() // A client can have multiple TaskRequests
                .HasForeignKey(tr => tr.ClientId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent deletion if related

            modelBuilder.Entity<TaskRequest>()
                .HasOne(tr => tr.Driver)
                .WithMany() // A driver can accept multiple TaskRequests
                .HasForeignKey(tr => tr.DriverId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Message entity
            modelBuilder.Entity<Message>()
                .HasKey(m => m.Id);

            modelBuilder.Entity<Message>()
                .Property(m => m.Content)
                .IsRequired()
                .HasMaxLength(1000);

            modelBuilder.Entity<Message>()
                .Property(m => m.SentAt)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Message>()
                .HasOne(m => m.TaskRequest)
                .WithMany() // A task can have multiple messages
                .HasForeignKey(m => m.TaskRequestId)
                .OnDelete(DeleteBehavior.Restrict);

            //// Configure DriverProfile entity
            //modelBuilder.Entity<DriverProfile>()
            //    .HasKey(dp => dp.Id);

            //modelBuilder.Entity<DriverProfile>()
            //    .HasOne(dp => dp.User)
            //    .WithOne()
            //    .HasForeignKey<DriverProfile>(dp => dp.UserId)
            //    .OnDelete(DeleteBehavior.Cascade); // If a user is deleted, their profile is deleted

            // Configure Stop entity (for multi-stop routes)
            modelBuilder.Entity<Stop>()
                .HasKey(s => s.Id);

            modelBuilder.Entity<Stop>()
                .Property(s => s.Latitude)
                .IsRequired();

            modelBuilder.Entity<Stop>()
                .Property(s => s.Longitude)
                .IsRequired();

            modelBuilder.Entity<Stop>()
                .Property(s => s.Order)
                .IsRequired();

            modelBuilder.Entity<Stop>()
                .HasOne(s => s.TaskRequest)
                .WithMany(tr => tr.Stops)
                .HasForeignKey(s => s.TaskRequestId)
                .OnDelete(DeleteBehavior.Cascade); // Stops are deleted if TaskRequest is deleted
        }
    }
}
