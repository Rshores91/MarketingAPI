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
    public Dictionary<string, int> InterestBreakdown { get; set; } = [];

    /// <summary>
    /// Next upcoming birthdays for quick reference.
    /// </summary>
    public List<UpcomingBirthdayDto> UpcomingBirthdays { get; set; } = [];
}