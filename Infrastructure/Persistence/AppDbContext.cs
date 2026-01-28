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

            // Add unique constraint for Voucher Code
            modelBuilder.Entity<Voucher>()
                .HasIndex(v => v.Code)
                .IsUnique();
        }
    }
}
