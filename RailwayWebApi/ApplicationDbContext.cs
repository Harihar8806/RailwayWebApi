using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RailwayWebApi.Models;

namespace RailwayWebApi.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Train> Trains { get; set; }
        public DbSet<TrainRoutes> TrainRoutes { get; set; }

        public DbSet<Booking> BOOKINGS { get; set; }

       public DbSet<Passenger> PASSENGERS { get; set; }

        public DbSet<Dailyseatavailable> DAILYSEATAVAILABLE { get; set; }
        public DbSet<TrainBetweenStation> TrainBetweenStations { get; set; }

        public DbSet<CalculateFare> CalculateFares { get; set; }

        public DbSet<Coaches> COACHES { get; set; }
        public DbSet<Station> STATIONS { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<TrainBetweenStation>().HasNoKey();
            builder.Entity<CalculateFare>().HasNoKey();
            builder.Entity<TrainRoutes>().Property(r => r.ROUTEID).ValueGeneratedOnAdd();

            base.OnModelCreating(builder);

            // Example: Map DateTimeOffset to DateTime
            builder.Entity<IdentityUser>(entity =>
            {
                entity.Property(e => e.UserName).HasMaxLength(256);
                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
                entity.Property(e => e.Email).HasMaxLength(256);
                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
                entity.Property(e => e.LockoutEnd)
                    .HasConversion(
                        v => v.HasValue ? v.Value.UtcDateTime : (DateTime?)null,
                        v => v.HasValue ? new DateTimeOffset(v.Value, TimeSpan.Zero) : (DateTimeOffset?)null
                    );

                entity.Property(e => e.EmailConfirmed).HasColumnType("NUMBER(1)");
                entity.Property(e => e.PhoneNumberConfirmed).HasColumnType("NUMBER(1)");
                entity.Property(e => e.TwoFactorEnabled).HasColumnType("NUMBER(1)");
                entity.Property(e => e.LockoutEnabled).HasColumnType("NUMBER(1)");
            });
            builder.Entity<IdentityRole>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(256);
                entity.Property(e => e.NormalizedName).HasMaxLength(256);
            });


        }

    }
}
