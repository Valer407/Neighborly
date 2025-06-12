using Microsoft.EntityFrameworkCore;
using Neighborly.Models.DBModels;

namespace Neighborly.Data
{
    public class NeighborlyContext : DbContext
    {
    public DbSet<User> Users { get; set; }
    public DbSet<Categories> Categories { get; set; }
    public DbSet<Listing_types> Listing_Types { get; set; }
    public DbSet<Cities> Cities { get; set; }
    public DbSet<Distircts> Districts { get; set; }
    public DbSet<Listings> Listings { get; set; }
    public DbSet<Listing_images> Listing_Images { get; set; }
    public DbSet<Chats> Chats { get; set; }
    public DbSet<Messages> Messages { get; set; }
    public DbSet<User_ratings> User_Ratings { get; set; }
    public DbSet<Reports> Reports { get; set; }
    public DbSet<Favourites> Favourites { get; set; }
    public DbSet<User_sessions> User_Sessions { get; set; }
    public DbSet<ListingView> ListingViews { get; set; }
    public DbSet<UserNotification> UserNotifications { get; set; }
    public DbSet<BlockedUser> BlockedUsers { get; set; }
    public DbSet<Feedback> Feedbacks { get; set; }
    public DbSet<ListingPromotion> ListingPromotions { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Role> Roles { get; set; }

        public NeighborlyContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Klucz główny złożony dla Favourites
            modelBuilder.Entity<Favourites>()
                .HasKey(f => new { f.UserId, f.ListingId });

            // Klucz główny złożony dla BlockedUser
            modelBuilder.Entity<BlockedUser>()
                .HasKey(b => new { b.UserId, b.BlockedId });

            // Konfiguracja relacji dla BlockedUser
            modelBuilder.Entity<BlockedUser>()
                .HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Można usunąć użytkownika

            modelBuilder.Entity<BlockedUser>()
                .HasOne(x => x.BlockedUserRef)
                .WithMany()
                .HasForeignKey(x => x.BlockedId)
                .OnDelete(DeleteBehavior.Restrict); // Nie usuwać zablokowanego automatycznie

            modelBuilder.Entity<Listings>()
                .HasOne(l => l.User)
                .WithMany(u => u.Listings)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Listings>()
                .HasOne(l => l.Category)
                .WithMany()
                .HasForeignKey(l => l.CategoryId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<Listings>()
                .HasOne(l => l.ListingType)
                .WithMany()
                .HasForeignKey(l => l.ListingTypeId)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<Listings>()
                .HasOne(l => l.City)
                .WithMany()
                .HasForeignKey(l => l.CityId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Listings>()
                .HasOne(l => l.District)
                .WithMany()
                .HasForeignKey(l => l.DistrictId)
                .OnDelete(DeleteBehavior.Restrict); 
            modelBuilder.Entity<Favourites>()
                .HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Restrict); // 👈 konieczne!

            modelBuilder.Entity<Favourites>()
                .HasOne(f => f.Listing)
                .WithMany()
                .HasForeignKey(f => f.ListingId)
                .OnDelete(DeleteBehavior.Cascade); // może pozostać, jeśli chcesz usuwać ogłoszenia z ulubionych
                
            base.OnModelCreating(modelBuilder);
        }
    }
}
