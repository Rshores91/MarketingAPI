namespace MarketingAPI.Models.DTOs;

/// <summary>
/// Data transfer object for sending an SMS.
/// </summary>
public class SmsDto
{
    /// <summary>
    /// The phone number of the recipient.
    /// </summary>
    public string Recipient { get; set; } = string.Empty;

    /// <summary>
    /// The message body.
    /// </summary>
    public string Message { get; set; } = string.Empty;
}