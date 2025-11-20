using RealEstatePlatform.Domain.Common;

namespace RealEstatePlatform.Domain.Entities;

/// <summary>
/// Blog category entity
/// </summary>
public class Category : BaseEntity
{
    /// <summary>
    /// Category name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Category slug for URL
    /// </summary>
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// Category description
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Parent category ID (for nested categories)
    /// </summary>
    public Guid? ParentId { get; set; }

    /// <summary>
    /// Display order
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Is category active
    /// </summary>
    public bool IsActive { get; set; }

    // Navigation properties
    public virtual Category? Parent { get; set; }
    public virtual ICollection<Category> Children { get; set; } = new List<Category>();
    public virtual ICollection<BlogPost> BlogPosts { get; set; } = new List<BlogPost>();
}
