using MarketingAPI.Data;
using MarketingAPI.Models;
using MarketingAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneNumbers;

namespace MarketingAPI.Controllers;

/*
===============================================================================================================================
This endpoint handles everything to do with the customer data.

Endpoints:
POST api/customer: Adds a customer's information to the database. Will reject if the customer already has an email associated.
GET api/customer: Gets a list of all the customers enrolled.
GET api/customer/{id}: Gets a customers information based on the provided id.
GET api/customer/by_email/{email}: Gets a customers information based on the provided email address.
DELETE api/customer/{id}: Deletes a customer's information based on the provided id.
DELETE api/customer/by_email/{email}: Deletes a customer's information based on the provided email address.

===============================================================================================================================
*/

/// <summary>
/// 
/// </summary>
/// <param name="context"></param>
/// <param name="logger"></param>
[ApiController]
[Route("api/[controller]")]
public class CustomersController(ApplicationDbContext context, ILogger<CustomersController> logger) : ControllerBase
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<CustomersController> _logger = logger;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="customerDto"></param>
    /// <returns></returns>
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult> AddUser([FromBody] CustomerDto customerDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (await _context.Customers.AnyAsync(c => c.Email == customerDto.Email))
            return BadRequest(new { message = "Email already registered" });

        PhoneNumberUtil phoneUtil = PhoneNumberUtil.GetInstance();
        PhoneNumber phoneNumber = phoneUtil.Parse(customerDto.PhoneNumber, "US");
        string formattedPhoneNumber = phoneUtil.Format(phoneNumber, PhoneNumberFormat.E164);

        if (await _context.Customers.AnyAsync(c => c.PhoneNumber == customerDto.PhoneNumber))
            return BadRequest(new { message = "Phone number already registered" });

        Customer customer = new()
        {
            Name = customerDto.Name,
            Email = customerDto.Email,
            PhoneNumber = formattedPhoneNumber,
            Birthday = customerDto.Birthday,
            Interest = customerDto.Interest,
            AgreeToSms = customerDto.AgreeToSms
        };

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Customer {customer.Name} was successfully registered.");

        return Ok();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAllCustomers()
    {
        List<CustomerDto> customers = await _context.Customers.Select(c => new CustomerDto
        {
            Name = c.Name,
            Email = c.Email,
            PhoneNumber = c.PhoneNumber,
            Birthday = c.Birthday,
            Interest = c.Interest,
            AgreeToSms = c.AgreeToSms
        }).ToListAsync();

        return Ok(customers);
    }

    /// <summary>
    /// Returns aggregated customer metrics for the manager dashboard.
    /// </summary>
    [HttpGet("metrics")]
    [Authorize]
    public async Task<ActionResult<CustomerMetricsDto>> GetCustomerMetrics()
    {
        List<Customer> customers = await _context.Customers.ToListAsync();

        CustomerMetricsDto metrics = new()
        {
            TotalCustomers = customers.Count
        };

        metrics.OptedInCustomers = customers.Count(c => c.AgreeToSms);
        metrics.OptedOutCustomers = metrics.TotalCustomers - metrics.OptedInCustomers;
        metrics.OptInRate = metrics.TotalCustomers == 0
            ? 0
            : Math.Round((double)metrics.OptedInCustomers / metrics.TotalCustomers * 100, 1);

        metrics.InterestBreakdown = customers
            .GroupBy(c => c.Interest)
            .OrderByDescending(g => g.Count())
            .ToDictionary(g => g.Key, g => g.Count());

        DateTime today = DateTime.Today;
        metrics.UpcomingBirthdays = customers
            .Select(c =>
            {
                int dayThisYear = Math.Min(c.Birthday.Day, DateTime.DaysInMonth(today.Year, c.Birthday.Month));
                DateTime nextBirthday = new(today.Year, c.Birthday.Month, dayThisYear);
                if (nextBirthday < today)
                {
                    int nextYear = today.Year + 1;
                    int adjustedDay = Math.Min(c.Birthday.Day, DateTime.DaysInMonth(nextYear, c.Birthday.Month));
                    nextBirthday = new DateTime(nextYear, c.Birthday.Month, adjustedDay);
                }

                return new UpcomingBirthdayDto
                {
                    Name = c.Name,
                    Interest = c.Interest,
                    Birthday = c.Birthday,
                    DaysUntilBirthday = (nextBirthday - today).Days
                };
            })
            .OrderBy(b => b.DaysUntilBirthday)
            .Take(5)
            .ToList();

        return Ok(metrics);
    }

    /// <summary>
    /// Returns the distinct list of customer interests for targeting.
    /// </summary>
    [HttpGet("interests")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<string>>> GetCustomerInterests()
    {
        List<string> interests = await _context.Customers
            .Where(c => !string.IsNullOrWhiteSpace(c.Interest))
            .Select(c => c.Interest)
            .Distinct()
            .OrderBy(i => i)
            .ToListAsync();

        return Ok(interests);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<CustomerDto>> GetCustomerById(int id)
    {
        Customer? customer = await _context.Customers.FindAsync(id);
        if (customer is null)
            return NotFound();

        CustomerDto customerDto = new()
        {
            Name = customer.Name,
            Email = customer.Email,
            PhoneNumber = customer.PhoneNumber,
            Birthday = customer.Birthday,
            Interest = customer.Interest,
            AgreeToSms = customer.AgreeToSms
        };

        return Ok(customerDto);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    [HttpGet("by_email/{email}")]
    [Authorize]
    public async Task<ActionResult<CustomerDto>> GetCustomerByEmail(string email)
    {
        Customer? customer = await _context.Customers.FirstAsync(c => c.Email == email);
        if (customer is null)
            return NotFound();

        CustomerDto customerDto = new()
        {
            Name = customer.Name,
            Email = customer.Email,
            PhoneNumber = customer.PhoneNumber,
            Birthday = customer.Birthday,
            Interest = customer.Interest,
            AgreeToSms = customer.AgreeToSms
        };

        return Ok(customerDto);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<ActionResult> DeleteCustomerById(int id)
    {
        Customer? customer = await _context.Customers.FindAsync(id);
        if (customer is null)
            return NotFound();

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Customer deleted: {customer.Name}");

        return NoContent();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    [HttpDelete("by_email/{email}")]
    [Authorize]
    public async Task<ActionResult<CustomerDto>> DeleteCustomerByEmail(string email)
    {
        Customer? customer = await _context.Customers.FirstAsync(c => c.Email == email);
        if (customer is null)
            return NotFound();

        _context.Customers.Remove(customer);
        await _context.SaveChangesAsync();

        _logger.LogInformation($"Customer deleted: {customer.Name}");

        return NoContent();
    }
}