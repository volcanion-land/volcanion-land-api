namespace RealEstatePlatform.Domain.Enums;

/// <summary>
/// Status of a payment transaction
/// </summary>
public enum PaymentStatus
{
    Pending = 1,
    Processing = 2,
    Completed = 3,
    Failed = 4,
    Refunded = 5,
    Cancelled = 6
}
