using Twilio.AspNet.Common;

namespace MarketingAPI.Models.DTOs;

public class BulkSmsDto : SmsRequest
{
    /// <summary>
    /// The formatted string for the message. Formatted as "Hi {name}, we are having a party on Friday!"
    /// </summary>
    public string MessageTemplate { get; set; } = string.Empty;

    /// <summary>
    /// The tags for the users that receive the message. This will use a tagType:URIIdentifier scheme. IE, event:live or date:birthday.
    /// </summary>
    public string RecipientTag { get; set; } = string.Empty;
}