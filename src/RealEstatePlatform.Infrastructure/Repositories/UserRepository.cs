using Microsoft.EntityFrameworkCore;
using RealEstatePlatform.Application.Interfaces.Repositories;
using RealEstatePlatform.Domain.Entities;
using RealEstatePlatform.Infrastructure.Data;

namespace RealEstatePlatform.Infrastructure.Repositories;

/// <summary>
/// User repository implementation
/// </summary>
public class UserRepository : Repository<ApplicationUser>, IUserRepository
{
    public UserRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<ApplicationUser?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.UserName == username, cancellationToken);
    }

    public async Task<bool> ExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(u => u.Email == email, cancellationToken);
    }
}
