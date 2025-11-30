using System;
using System.Collections.Generic;

namespace MarketingAPI.Models.DTOs;

/// <summary>
/// Aggregated metrics about customers for dashboard display.
/// </summary>
public class CustomerMetricsDto
{
    /// <summary>
    /// Total number of customers stored in the system.
    /// </summary>
    public int TotalCustomers { get; set; }

    /// <summary>
    /// Count of customers that opted into SMS notifications.
    /// </summary>
    public int OptedInCustomers { get; set; }

    /// <summary>
    /// Count of customers that are not opted into SMS notifications.
    /// </summary>
    public int OptedOutCustomers { get; set; }

    /// <summary>
    /// Percentage of customers that opted into SMS notifications.
    /// </summary>
    public double OptInRate { get; set; }

    /// <summary>
    /// Breakdown of customers by interest tag.
    /// </summary>
    public Dictionary<string, int> InterestBreakdown { get; set; } = new();

    /// <summary>
    /// Next upcoming birthdays for quick reference.
    /// </summary>
    public List<UpcomingBirthdayDto> UpcomingBirthdays { get; set; } = [];
}

/// <summary>
/// DTO representing an upcoming customer birthday.
/// </summary>
public class UpcomingBirthdayDto
{
    public string Name { get; set; } = string.Empty;
    public string Interest { get; set; } = string.Empty;
    public DateTime Birthday { get; set; }
    public int DaysUntilBirthday { get; set; }
}
