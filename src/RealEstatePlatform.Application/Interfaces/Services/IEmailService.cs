namespace RealEstatePlatform.Application.Interfaces.Services;

/// <summary>
/// Email service interface
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Send email confirmation email
    /// </summary>
    Task SendEmailConfirmationAsync(string email, string name, string confirmationToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Send password reset email
    /// </summary>
    Task SendPasswordResetAsync(string email, string name, string resetToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Send welcome email
    /// </summary>
    Task SendWelcomeEmailAsync(string email, string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Send contact request notification to admin
    /// </summary>
    Task SendContactRequestNotificationAsync(string adminEmail, string userName, string subject, string message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Send contact request response to user
    /// </summary>
    Task SendContactResponseAsync(string email, string name, string subject, string response, CancellationToken cancellationToken = default);

    /// <summary>
    /// Send listing status notification
    /// </summary>
    Task SendListingStatusChangeAsync(string email, string name, string listingTitle, string status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Send new matching listing notification
    /// </summary>
    Task SendNewMatchingListingAsync(string email, string name, string listingTitle, string listingUrl, CancellationToken cancellationToken = default);

    /// <summary>
    /// Send payment confirmation email
    /// </summary>
    Task SendPaymentConfirmationAsync(string email, string name, string transactionId, decimal amount, CancellationToken cancellationToken = default);

    /// <summary>
    /// Send general email
    /// </summary>
    Task SendEmailAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default);
}
