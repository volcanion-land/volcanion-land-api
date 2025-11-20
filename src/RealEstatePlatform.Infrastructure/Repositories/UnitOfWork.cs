using Microsoft.EntityFrameworkCore.Storage;
using RealEstatePlatform.Application.Interfaces.Repositories;
using RealEstatePlatform.Domain.Entities;
using RealEstatePlatform.Infrastructure.Data;
using System.Collections;

namespace RealEstatePlatform.Infrastructure.Repositories;

/// <summary>
/// Unit of work implementation
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;
    private Hashtable? _repositories;

    private IPropertyListingRepository? _propertyListings;
    private IUserRepository? _users;
    private IRepository<Notification>? _notifications;
    private IRepository<FavoriteListing>? _favoriteListings;
    private IRepository<Message>? _messages;
    private IRepository<Conversation>? _conversations;
    private IRepository<ConversationParticipant>? _conversationParticipants;
    private IRepository<Review>? _reviews;
    private IRepository<UserFollowing>? _userFollowings;
    private IRepository<SavedSearch>? _savedSearches;
    private IRepository<ContactRequest>? _contactRequests;
    private IRepository<Banner>? _banners;
    private IRepository<FAQ>? _faqs;
    private IRepository<SystemConfiguration>? _systemConfigurations;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IPropertyListingRepository PropertyListings
    {
        get
        {
            _propertyListings ??= new PropertyListingRepository(_context);
            return _propertyListings;
        }
    }

    public IUserRepository Users
    {
        get
        {
            _users ??= new UserRepository(_context);
            return _users;
        }
    }

    public IRepository<Notification> Notifications
    {
        get
        {
            _notifications ??= new Repository<Notification>(_context);
            return _notifications;
        }
    }

    public IRepository<FavoriteListing> FavoriteListings
    {
        get
        {
            _favoriteListings ??= new Repository<FavoriteListing>(_context);
            return _favoriteListings;
        }
    }

    public IRepository<Message> Messages
    {
        get
        {
            _messages ??= new Repository<Message>(_context);
            return _messages;
        }
    }

    public IRepository<Conversation> Conversations
    {
        get
        {
            _conversations ??= new Repository<Conversation>(_context);
            return _conversations;
        }
    }

    public IRepository<ConversationParticipant> ConversationParticipants
    {
        get
        {
            _conversationParticipants ??= new Repository<ConversationParticipant>(_context);
            return _conversationParticipants;
        }
    }

    public IRepository<Review> Reviews
    {
        get
        {
            _reviews ??= new Repository<Review>(_context);
            return _reviews;
        }
    }

    public IRepository<UserFollowing> UserFollowings
    {
        get
        {
            _userFollowings ??= new Repository<UserFollowing>(_context);
            return _userFollowings;
        }
    }

    public IRepository<SavedSearch> SavedSearches
    {
        get
        {
            _savedSearches ??= new Repository<SavedSearch>(_context);
            return _savedSearches;
        }
    }

    public IRepository<ContactRequest> ContactRequests
    {
        get
        {
            _contactRequests ??= new Repository<ContactRequest>(_context);
            return _contactRequests;
        }
    }

    public IRepository<Banner> Banners
    {
        get
        {
            _banners ??= new Repository<Banner>(_context);
            return _banners;
        }
    }

    public IRepository<FAQ> FAQs
    {
        get
        {
            _faqs ??= new Repository<FAQ>(_context);
            return _faqs;
        }
    }

    public IRepository<SystemConfiguration> SystemConfigurations
    {
        get
        {
            _systemConfigurations ??= new Repository<SystemConfiguration>(_context);
            return _systemConfigurations;
        }
    }

    public IRepository<T> Repository<T>() where T : class
    {
        _repositories ??= new Hashtable();

        var type = typeof(T).Name;

        if (!_repositories.ContainsKey(type))
        {
            var repositoryType = typeof(Repository<>);
            var repositoryInstance = Activator.CreateInstance(
                repositoryType.MakeGenericType(typeof(T)), _context);
            _repositories.Add(type, repositoryInstance);
        }

        return (IRepository<T>)_repositories[type]!;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
