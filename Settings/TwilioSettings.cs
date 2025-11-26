namespace MarketingAPI.Settings;

public class TwilioSettings
{
    public const string SectionName = "Twilio";

    public string AccountSid { get; set; } = string.Empty;
    public string AuthToken { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string MessagingServiceSid { get; set; } = string.Empty;
    public bool EnableRequestValidation { get; set; } = true;
    public string DefaultCountryCode { get; set; } = "+1";

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