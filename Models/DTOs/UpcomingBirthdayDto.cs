namespace MarketingAPI.Models.DTOs;

/// <summary>
/// DTO representing an upcoming customer birthday.
/// </summary>
public class UpcomingBirthdayDto
{
    /// <summary>
    /// The name of the customer
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The interest of the customer
    /// </summary>
    public string Interest { get; set; } = string.Empty;

    /// <summary>
    /// The customers birthday
    /// </summary>
    public DateTime Birthday { get; set; }

    /// <summary>
    /// The amount of days until the customers birthday
    /// </summary>
    public int DaysUntilBirthday { get; set; }
}
