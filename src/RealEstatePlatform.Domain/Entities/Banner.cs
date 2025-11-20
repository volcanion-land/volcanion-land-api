using RealEstatePlatform.Domain.Common;
using RealEstatePlatform.Domain.Enums;

namespace RealEstatePlatform.Domain.Entities;

/// <summary>
/// Banner/Advertisement on the website
/// </summary>
public class Banner : BaseEntity
{
    /// <summary>
    /// Banner title
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Banner image URL
    /// </summary>
    public required string ImageUrl { get; set; }

    /// <summary>
    /// Link when banner is clicked
    /// </summary>
    public string? LinkUrl { get; set; }

    /// <summary>
    /// Banner position (homepage_slider, sidebar, header, footer)
    /// </summary>
    public required string Position { get; set; }

    /// <summary>
    /// Display order
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Start date for displaying banner
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// End date for displaying banner
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Is banner currently active
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Click count
    /// </summary>
    public int ClickCount { get; set; }

    /// <summary>
    /// View count
    /// </summary>
    public int ViewCount { get; set; }

    /// <summary>
    /// Target audience (all, mobile, desktop)
    /// </summary>
    public string Target { get; set; } = "all";

    /// <summary>
    /// Banner description
    /// </summary>
    public string? Description { get; set; }
}
