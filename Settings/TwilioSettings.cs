namespace MarketingAPI.Settings;

/// <summary>
/// Configuration settings for Twilio integration.
/// </summary>
public class TwilioSettings
{
    /// <summary>
    /// The configuration section name.
    /// </summary>
    public const string SectionName = "Twilio";

    /// <summary>
    /// The Twilio Account SID.
    /// </summary>
    public string AccountSid { get; set; } = string.Empty;

    /// <summary>
    /// The Twilio Auth Token.
    /// </summary>
    public string AuthToken { get; set; } = string.Empty;

    /// <summary>
    /// The Twilio Phone Number.
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// The Twilio Messaging Service SID.
    /// </summary>
    public string MessagingServiceSid { get; set; } = string.Empty;

    /// <summary>
    /// Whether to enable request validation.
    /// </summary>
    public bool EnableRequestValidation { get; set; } = true;

    /// <summary>
    /// The default country code for phone numbers.
    /// </summary>
    public string DefaultCountryCode { get; set; } = "+1";

    /// <summary>
    /// Validates the Twilio settings.
    /// </summary>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(AccountSid))
            throw new InvalidOperationException("Twilio:AccountSid is not configured. Please set it in User Secrets.");

        if (string.IsNullOrWhiteSpace(AuthToken))
            throw new InvalidOperationException("Twilio:AuthToken is not configured. Please set it in User Secrets.");

        if (!AccountSid.StartsWith("AC"))
            throw new InvalidOperationException("Twilio:AccountSid must start with 'AC'.");
    }
}