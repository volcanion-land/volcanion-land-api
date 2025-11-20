namespace RealEstatePlatform.Application.DTOs.Admin;

public class SystemConfigDto
{
    public Guid Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? Group { get; set; }
    public string? Description { get; set; }
    public string DataType { get; set; } = "string";
    public bool IsVisible { get; set; }
    public bool IsSystem { get; set; }
}

public class UpdateSystemConfigDto
{
    public string Value { get; set; } = string.Empty;
}

public class BannerDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string? LinkUrl { get; set; }
    public string Position { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }
    public int ClickCount { get; set; }
    public int ViewCount { get; set; }
    public string Target { get; set; } = "all";
    public string? Description { get; set; }
}

public class CreateBannerDto
{
    public string Title { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string? LinkUrl { get; set; }
    public string Position { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; } = true;
    public string Target { get; set; } = "all";
    public string? Description { get; set; }
}

public class UpdateBannerDto
{
    public string Title { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string? LinkUrl { get; set; }
    public string Position { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool IsActive { get; set; }
    public string Target { get; set; } = "all";
    public string? Description { get; set; }
}

public class FAQDto
{
    public Guid Id { get; set; }
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public string? Category { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsPublished { get; set; }
    public int ViewCount { get; set; }
    public int HelpfulCount { get; set; }
    public int NotHelpfulCount { get; set; }
}

public class CreateFAQDto
{
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public string? Category { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsPublished { get; set; } = true;
}

public class UpdateFAQDto
{
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public string? Category { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsPublished { get; set; }
}

public class DashboardStatsDto
{
    public int TotalUsers { get; set; }
    public int TotalListings { get; set; }
    public int ActiveListings { get; set; }
    public int PendingListings { get; set; }
    public int SoldListings { get; set; }
    public int TotalViews { get; set; }
    public decimal TotalRevenue { get; set; }
    public int NewUsersThisMonth { get; set; }
    public int NewListingsThisMonth { get; set; }
    public List<MonthlyStatDto>? MonthlyStats { get; set; }
    public List<PopularLocationDto>? PopularLocations { get; set; }
    public List<TopListingDto>? TopListings { get; set; }
}

public class MonthlyStatDto
{
    public string Month { get; set; } = string.Empty;
    public int ListingCount { get; set; }
    public int UserCount { get; set; }
    public decimal Revenue { get; set; }
}

public class PopularLocationDto
{
    public string Location { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class TopListingDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int ViewCount { get; set; }
    public decimal Price { get; set; }
}
