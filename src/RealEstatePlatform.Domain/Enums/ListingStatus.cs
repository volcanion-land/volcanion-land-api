namespace RealEstatePlatform.Domain.Enums;

/// <summary>
/// Status of a property listing
/// </summary>
public enum ListingStatus
{
    Draft = 0,
    Active = 1,
    Pending = 2,
    Sold = 3,
    Rented = 4,
    Expired = 5,
    Suspended = 6,
    Cancelled = 7
}
