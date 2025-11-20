using RealEstatePlatform.Domain.Common;

namespace RealEstatePlatform.Domain.Entities;

/// <summary>
/// Blog post entity
/// </summary>
public class BlogPost : BaseEntity
{
    /// <summary>
    /// Post title
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Post slug for URL
    /// </summary>
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// Post excerpt/summary
    /// </summary>
    public string? Excerpt { get; set; }

    /// <summary>
    /// Post content
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Featured image URL
    /// </summary>
    public string? FeaturedImageUrl { get; set; }

    /// <summary>
    /// Author user ID
    /// </summary>
    public string AuthorId { get; set; } = string.Empty;

    /// <summary>
    /// Category ID
    /// </summary>
    public Guid CategoryId { get; set; }

    /// <summary>
    /// View count
    /// </summary>
    public int ViewCount { get; set; }

    /// <summary>
    /// Is published
    /// </summary>
    public bool IsPublished { get; set; }

    /// <summary>
    /// Published at timestamp
    /// </summary>
    public DateTime? PublishedAt { get; set; }

    /// <summary>
    /// Meta title for SEO
    /// </summary>
    public string? MetaTitle { get; set; }

    /// <summary>
    /// Meta description for SEO
    /// </summary>
    public string? MetaDescription { get; set; }

    /// <summary>
    /// Meta keywords for SEO
    /// </summary>
    public string? MetaKeywords { get; set; }

    // Navigation properties
    public virtual ApplicationUser Author { get; set; } = null!;
    public virtual Category Category { get; set; } = null!;
}
