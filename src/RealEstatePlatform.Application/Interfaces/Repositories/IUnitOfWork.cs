using RealEstatePlatform.Domain.Entities;

namespace RealEstatePlatform.Application.Interfaces.Repositories;

/// <summary>
/// Unit of work interface
/// </summary>
public interface IUnitOfWork : IDisposable
{
    IPropertyListingRepository PropertyListings { get; }
    IUserRepository Users { get; }
    IRepository<Notification> Notifications { get; }
    IRepository<FavoriteListing> FavoriteListings { get; }
    IRepository<Message> Messages { get; }
    IRepository<Conversation> Conversations { get; }
    IRepository<ConversationParticipant> ConversationParticipants { get; }
    IRepository<Review> Reviews { get; }
    IRepository<UserFollowing> UserFollowings { get; }
    IRepository<SavedSearch> SavedSearches { get; }
    IRepository<ContactRequest> ContactRequests { get; }
    IRepository<Banner> Banners { get; }
    IRepository<FAQ> FAQs { get; }
    IRepository<SystemConfiguration> SystemConfigurations { get; }
    IRepository<T> Repository<T>() where T : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
