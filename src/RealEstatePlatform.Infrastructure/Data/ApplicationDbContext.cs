using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RealEstatePlatform.Domain.Entities;
using RealEstatePlatform.Infrastructure.Data.Configurations;
using System.Reflection;

namespace RealEstatePlatform.Infrastructure.Data;

/// <summary>
/// Application database context
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    private readonly string? _currentUserId;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IHttpContextAccessor httpContextAccessor)
        : base(options)
    {
        _currentUserId = httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;
    }

    // DbSets for all entities
    public virtual DbSet<Property> Properties => Set<Property>();
    public virtual DbSet<PropertyImage> PropertyImages => Set<PropertyImage>();
    public virtual DbSet<PropertyListing> PropertyListings => Set<PropertyListing>();
    public virtual DbSet<Amenity> Amenities => Set<Amenity>();
    public virtual DbSet<PropertyAmenity> PropertyAmenities => Set<PropertyAmenity>();
    public virtual DbSet<Province> Provinces => Set<Province>();
    public virtual DbSet<District> Districts => Set<District>();
    public virtual DbSet<Ward> Wards => Set<Ward>();
    public virtual DbSet<FavoriteListing> FavoriteListings => Set<FavoriteListing>();
    public virtual DbSet<ViewHistory> ViewHistories => Set<ViewHistory>();
    public virtual DbSet<Conversation> Conversations => Set<Conversation>();
    public virtual DbSet<ConversationParticipant> ConversationParticipants => Set<ConversationParticipant>();
    public virtual DbSet<Message> Messages => Set<Message>();
    public virtual DbSet<Notification> Notifications => Set<Notification>();
    public virtual DbSet<NotificationTemplate> NotificationTemplates => Set<NotificationTemplate>();
    public virtual DbSet<ListingPackage> ListingPackages => Set<ListingPackage>();
    public virtual DbSet<PaymentTransaction> PaymentTransactions => Set<PaymentTransaction>();
    public virtual DbSet<Category> Categories => Set<Category>();
    public virtual DbSet<BlogPost> BlogPosts => Set<BlogPost>();
    public virtual DbSet<Review> Reviews => Set<Review>();
    public virtual DbSet<SystemConfiguration> SystemConfigurations => Set<SystemConfiguration>();
    public virtual DbSet<Banner> Banners => Set<Banner>();
    public virtual DbSet<FAQ> FAQs => Set<FAQ>();
    public virtual DbSet<ContactRequest> ContactRequests => Set<ContactRequest>();
    public virtual DbSet<UserFollowing> UserFollowings => Set<UserFollowing>();
    public virtual DbSet<SavedSearch> SavedSearches => Set<SavedSearch>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Enable PostGIS extension for spatial data
        modelBuilder.HasPostgresExtension("postgis");

        // Apply all entity configurations from current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Rename Identity tables to remove AspNet prefix
        modelBuilder.Entity<ApplicationUser>().ToTable("Users");
        modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityRole>().ToTable("Roles");
        modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserRole<string>>().ToTable("UserRoles");
        modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserClaim<string>>().ToTable("UserClaims");
        modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserLogin<string>>().ToTable("UserLogins");
        modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityUserToken<string>>().ToTable("UserTokens");
        modelBuilder.Entity<Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>>().ToTable("RoleClaims");
    }

    /// <summary>
    /// Override SaveChanges to automatically set audit fields
    /// </summary>
    public override int SaveChanges()
    {
        UpdateAuditFields();
        return base.SaveChanges();
    }

    /// <summary>
    /// Override SaveChangesAsync to automatically set audit fields
    /// </summary>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        return await base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Update audit fields for entities
    /// </summary>
    private void UpdateAuditFields()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.Entity is Domain.Common.BaseEntity &&
                       (e.State == EntityState.Added || e.State == EntityState.Modified));

        var now = DateTime.UtcNow;

        foreach (var entry in entries)
        {
            var entity = (Domain.Common.BaseEntity)entry.Entity;

            if (entry.State == EntityState.Added)
            {
                entity.CreatedAt = now;
                entity.CreatedBy = _currentUserId;
            }

            if (entry.State == EntityState.Modified)
            {
                entity.UpdatedAt = now;
                entity.UpdatedBy = _currentUserId;
            }
        }

        // Handle ApplicationUser separately since it doesn't inherit from BaseEntity
        var userEntries = ChangeTracker.Entries<ApplicationUser>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in userEntries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
            }
        }
    }
}
