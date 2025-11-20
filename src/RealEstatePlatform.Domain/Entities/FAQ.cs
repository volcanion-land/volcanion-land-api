using RealEstatePlatform.Domain.Common;

namespace RealEstatePlatform.Domain.Entities;

/// <summary>
/// Frequently Asked Questions
/// </summary>
public class FAQ : BaseEntity
{
    /// <summary>
    /// Question
    /// </summary>
    public required string Question { get; set; }

    /// <summary>
    /// Answer
    /// </summary>
    public required string Answer { get; set; }

    /// <summary>
    /// Category (general, listing, payment, account)
    /// </summary>
    public string? Category { get; set; }

    /// <summary>
    /// Display order
    /// </summary>
    public int DisplayOrder { get; set; }

    /// <summary>
    /// Is this FAQ published
    /// </summary>
    public bool IsPublished { get; set; } = true;

    /// <summary>
    /// View count
    /// </summary>
    public int ViewCount { get; set; }

    /// <summary>
    /// Helpful count
    /// </summary>
    public int HelpfulCount { get; set; }

    /// <summary>
    /// Not helpful count
    /// </summary>
    public int NotHelpfulCount { get; set; }
}
