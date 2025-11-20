using RealEstatePlatform.Domain.Entities;

namespace RealEstatePlatform.Application.Interfaces.Repositories;

/// <summary>
/// User repository interface
/// </summary>
public interface IUserRepository : IRepository<ApplicationUser>
{
    Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<ApplicationUser?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string email, CancellationToken cancellationToken = default);
}
