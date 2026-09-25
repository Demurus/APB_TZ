using IlliaUlianych_APB_TZ.Entities;
using Microsoft.EntityFrameworkCore;

namespace IlliaUlianych_APB_TZ.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<ConferenceRoom> ConferenceRooms => Set<ConferenceRoom>();
    public DbSet<RoomService> RoomServices => Set<RoomService>();
    public DbSet<Booking> Bookings => Set<Booking>();

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Booking>()
            .HasOne(booking => booking.Room)
            .WithMany()
            .HasForeignKey(booking => booking.RoomId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ConferenceRoom>()
            .HasMany(room => room.AvailableServices)
            .WithMany()
            .UsingEntity(join =>
                join.ToTable("ConferenceRoomServices"));

        modelBuilder.Entity<Booking>()
            .HasMany(booking => booking.SelectedServices)
            .WithMany()
            .UsingEntity(join =>
                join.ToTable("BookingServices"));
        
        modelBuilder.Entity<RoomService>().HasData(
            new
            {
                Id = 1,
                Name = "Projector",
                Price = 500m
            },
            new
            {
                Id = 2,
                Name = "Wi-Fi",
                Price = 300m
            },
            new
            {
                Id = 3,
                Name = "Sound",
                Price = 700m
            });

        modelBuilder.Entity<ConferenceRoom>().HasData(
            new
            {
                Id = 1,
                Name = "Room A",
                Capacity = 50,
                BaseHourPrice = 2000m
            },
            new
            {
                Id = 2,
                Name = "Room B",
                Capacity = 100,
                BaseHourPrice = 3500m
            },
            new
            {
                Id = 3,
                Name = "Room C",
                Capacity = 30,
                BaseHourPrice = 1500m
            });
    }
}