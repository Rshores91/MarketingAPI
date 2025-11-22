using System.ComponentModel.DataAnnotations;

namespace MarketingAPI.Models.DTOs;

public class CustomerDto
{
    /// <summary>
    /// The customers name
    /// </summary>
    [Required]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The customers email. Max length of 200 characters.
    /// </summary>
    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// The customers phone number.
    /// </summary>
    public string PhoneNumber { get; set; } = string.Empty;

    /// <summary>
    /// The customers birthday.
    /// </summary>
    public DateTime Birthday { get; set; }

    /// <summary>
    /// The customers primary interest.
    /// </summary>
    public string Interest { get; set; } = string.Empty;

    /// <summary>
    /// Whether or not the customer agreed to receive SMS notifications.
    /// </summary>
    public bool AgreeToSms { get; set; } = false;
}