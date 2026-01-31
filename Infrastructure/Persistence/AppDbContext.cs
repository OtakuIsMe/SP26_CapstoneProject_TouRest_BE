using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TouRest.Domain.Entities;

namespace TouRest.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Image> Images => Set<Image>();
        public DbSet<Provider> Providers => Set<Provider>();
        public DbSet<ProviderUser> ProviderUsers => Set<ProviderUser>();
        public DbSet<Package> Packages => Set<Package>();
        public DbSet<Service> Services => Set<Service>();
        public DbSet<PackageService> PackageServices => Set<PackageService>();
        public DbSet<Itinerary> Itineraries => Set<Itinerary>();
        public DbSet<ItineraryStop> ItineraryStops => Set<ItineraryStop>();
        public DbSet<ItineraryActivity> ItineraryActivities => Set<ItineraryActivity>();
        public DbSet<Booking> Bookings => Set<Booking>();
        public DbSet<BookingItinerary> BookingItineraries => Set<BookingItinerary>();
        public DbSet<Voucher> Vouchers => Set<Voucher>();
        public DbSet<Refund> Refunds => Set<Refund>();
        public DbSet<Feedback> Feedbacks => Set<Feedback>();
        public DbSet<Report> Reports => Set<Report>();
        public DbSet<Wishlist> Wishlists => Set<Wishlist>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User - Role relationship
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure User - Image relationship
            modelBuilder.Entity<User>()
                .HasOne(u => u.Image)
                .WithOne(i => i.Users)
                .HasForeignKey<User>(u => u.ImageId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure Booking - User relationship
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany()
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Itinerary - User (Agency) relationship
            modelBuilder.Entity<Itinerary>()
                .HasOne(i => i.Agency)
                .WithMany()
                .HasForeignKey(i => i.AgencyId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure ItineraryStop - Itinerary relationship
            modelBuilder.Entity<ItineraryStop>()
                .HasOne(s => s.Itinerary)
                .WithMany()
                .HasForeignKey(s => s.ItineraryId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure ItineraryActivity - ItineraryStop relationship
            modelBuilder.Entity<ItineraryActivity>()
                .HasOne(a => a.ItineraryStop)
                .WithMany()
                .HasForeignKey(a => a.ItineraryStopId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure ItineraryActivity - Service relationship
            modelBuilder.Entity<ItineraryActivity>()
                .HasOne(a => a.Service)
                .WithMany()
                .HasForeignKey(a => a.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure BookingItinerary relationships
            modelBuilder.Entity<BookingItinerary>()
                .HasOne(bi => bi.Booking)
                .WithMany()
                .HasForeignKey(bi => bi.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BookingItinerary>()
                .HasOne(bi => bi.Itinerary)
                .WithMany()
                .HasForeignKey(bi => bi.ItineraryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<BookingItinerary>()
                .HasOne(bi => bi.Voucher)
                .WithMany()
                .HasForeignKey(bi => bi.VoucherId)
                .OnDelete(DeleteBehavior.SetNull);

            // Configure Service - Provider relationship
            modelBuilder.Entity<Service>()
                .HasOne(s => s.Provider)
                .WithMany()
                .HasForeignKey(s => s.ProviderId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure PackageService (many-to-many)
            modelBuilder.Entity<PackageService>()
                .HasKey(ps => new { ps.PackageId, ps.ServiceId });

            modelBuilder.Entity<PackageService>()
                .HasOne(ps => ps.Package)
                .WithMany()
                .HasForeignKey(ps => ps.PackageId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PackageService>()
                .HasOne(ps => ps.Service)
                .WithMany()
                .HasForeignKey(ps => ps.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure ProviderUser (many-to-many)
            modelBuilder.Entity<ProviderUser>()
                .HasOne(pu => pu.Provider)
                .WithMany()
                .HasForeignKey(pu => pu.ProviderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProviderUser>()
                .HasOne(pu => pu.User)
                .WithMany()
                .HasForeignKey(pu => pu.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Feedback - Booking relationship
            modelBuilder.Entity<Feedback>()
                .HasOne(f => f.Booking)
                .WithMany()
                .HasForeignKey(f => f.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Refund - Booking relationship
            modelBuilder.Entity<Refund>()
                .HasOne(r => r.Booking)
                .WithMany()
                .HasForeignKey(r => r.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Wishlist - User relationship
            modelBuilder.Entity<Wishlist>()
                .HasOne(w => w.User)
                .WithMany()
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Notification - User relationship
            modelBuilder.Entity<Notification>()
                .HasOne(n => n.RecipientUser)
                .WithMany()
                .HasForeignKey(n => n.RecipientUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Report - User relationship
            modelBuilder.Entity<Report>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure RefreshToken - User relationship
            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.User)
                .WithMany()
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ============= UNIQUE CONSTRAINTS =============

            // Unique constraint for Role Code
            modelBuilder.Entity<Role>()
                .HasIndex(r => r.Code)
                .IsUnique();

            // Unique constraint for User Email
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Unique constraint for Voucher Code
            modelBuilder.Entity<Voucher>()
                .HasIndex(v => v.Code)
                .IsUnique();

            // Unique constraint for Package Code
            modelBuilder.Entity<Package>()
                .HasIndex(p => p.Code)
                .IsUnique();

            // Unique constraint for Booking Code
            modelBuilder.Entity<Booking>()
                .HasIndex(b => b.Code)
                .IsUnique();

            // Unique constraint for Provider ContactEmail
            modelBuilder.Entity<Provider>()
                .HasIndex(p => p.ContactEmail)
                .IsUnique();

            // Unique constraint for Wishlist (ItemId, UserId)
            modelBuilder.Entity<Wishlist>()
                .HasIndex(w => new { w.ItemId, w.UserId })
                .IsUnique();

            // Unique constraint for BookingItinerary (BookingId, ItineraryId)
            modelBuilder.Entity<BookingItinerary>()
                .HasIndex(bi => new { bi.BookingId, bi.ItineraryId })
                .IsUnique();

            // Unique constraint for PackageService (PackageId, SortOrder)
            modelBuilder.Entity<PackageService>()
                .HasIndex(ps => new { ps.PackageId, ps.SortOrder })
                .IsUnique();

            // Unique constraint for ItineraryStop (ItineraryId, StopOrder)
            modelBuilder.Entity<ItineraryStop>()
                .HasIndex(ist => new { ist.ItineraryId, ist.StopOrder })
                .IsUnique();

            // Unique constraint for ItineraryActivity (ItineraryStopId, ActivityOrder)
            modelBuilder.Entity<ItineraryActivity>()
                .HasIndex(ia => new { ia.ItineraryStopId, ia.ActivityOrder })
                .IsUnique();

            // ============= SEED DATA =============

            // Seed default roles
            modelBuilder.Entity<Role>().HasData(
                new Role
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Code = "CUSTOMER",
                    Name = "Khách hàng",
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Role
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Code = "ADMIN",
                    Name = "Quản trị viên",
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Role
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Code = "PROVIDER",
                    Name = "Nhà cung cấp dịch vụ",
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Role
                {
                    Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                    Code = "AGENCY",
                    Name = "Đại lý du lịch",
                    CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}
