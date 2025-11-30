using MarketingAPI.Data;
using MarketingAPI.Models;
using MarketingAPI.Models.DTOs;
using MarketingAPI.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Twilio.AspNet.Common;
using Twilio.AspNet.Core;
using Twilio.Rest.Api.V2010.Account;
using Twilio.TwiML;
using Twilio.Types;

namespace MarketingAPI.Controllers;

/// <summary>
/// 
/// </summary>
/// <param name="context"></param>
/// <param name="logger"></param>
/// <param name="twilioSettings"></param>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SmsController(ApplicationDbContext context, ILogger<UsersController> logger, TwilioSettings twilioSettings) : ControllerBase
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<UsersController> _logger = logger;
    private readonly TwilioSettings _twilioSettings = twilioSettings;

    /// <summary>
    /// Sends a message to a single recipient.
    /// </summary>
    /// <param name="request">The SMS request object.</param>
    /// <returns>Object code result that corresponds to the code send to the client.</returns>
    [HttpPost("send")]
    public async Task<IActionResult> SendSms([FromBody] SmsDto request)
    {
        try
        {
            MessageResource message = await MessageResource.CreateAsync(
                to: new PhoneNumber(request.Recipient),
                from: new PhoneNumber(_twilioSettings.PhoneNumber),
                body: request.Message
            );

            _logger.LogInformation($"SMS sent successfully. SID: {message.Sid}");

            return Ok(new
            {
                success = true,
                messageSid = message.Sid,
                status = message.Status.ToString()
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to send message");
            return BadRequest(new { success = false, error = e.Message });
        }
    }

    /// <summary>
    /// Sends bulk messages to a group of people based on the DTO tag received.
    /// </summary>
    /// <param name="dto">The DTO object send with the request.</param>
    /// <returns></returns>
    [HttpPost("bulk")]
    public async Task<IActionResult> SendBulkSms([FromBody] BulkSmsDto dto)
    {
        string type = dto.RecipientTag.Split(':')[0];
        string evt = dto.RecipientTag.Split(':')[1];

        List<Customer> customers = type switch
        {
            "event" => [.. _context.Customers.Where(c => c.Interest == evt)],
            "date" => [.. _context.Customers.Where(c => false)],
            _ => [.. _context.Customers]
        };

        if (customers.Count == 0)
            return BadRequest($"There are no customers that match the tag {dto.RecipientTag}");

        try
        {
            List<MessageResult> results = [];
            foreach (Customer customer in customers)
            {
                try
                {
                    MessageResource message = await MessageResource.CreateAsync(
                        body: ResolveTemplate(dto.MessageTemplate, customer),
                        messagingServiceSid: _twilioSettings.MessagingServiceSid,
                        to: new PhoneNumber(customer.PhoneNumber)
                    );

                    results.Add(new()
                    {
                        PhoneNumber = customer.PhoneNumber,
                        MessageSid = message.Sid,
                        Status = message.Status.ToString(),
                        Success = true
                    });

                    _logger.LogInformation($"Message sent to {customer.PhoneNumber}: {message.Sid}");
                }
                catch (Exception e)
                {
                    results.Add(new MessageResult
                    {
                        PhoneNumber = customer.PhoneNumber,
                        Success = false,
                        Error = e.Message
                    });

                    _logger.LogError(e, $"Failed to send message to {customer.PhoneNumber}");
                }
            }

            return Ok(new
            {
                totalRecipients = customers.Count,
                successful = results.Count(r => r.Success),
                failed = results.Count(r => !r.Success),
                results
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Bulk SMS Operation failed.");
            return BadRequest(new { error = e.Message });
        }
    }
    
    
    /// <summary>
    /// Receives an incoming SMS message from Twilio.
    /// </summary>
    /// <param name="dto">The SMS request data.</param>
    /// <returns>The TwiML response.</returns>
    [HttpPost("sms")]
    [ValidateRequest]
    public TwiMLResult ReceiveMessage(SmsRequest dto)
    {
        MessagingResponse response = new();
        return new(response);
    }

    private string ResolveTemplate(string template, Customer customer)
    {
        return template.Replace("{name}", customer.Name);
    }
}

/// <summary>
/// Results of a message being sent
/// </summary>
public struct MessageResult
{
    /// <summary>
    /// The phone number the message was sent to.
    /// </summary>
    public string PhoneNumber { get; set; }

    /// <summary>
    /// The Twilio Message SID.
    /// </summary>
    public string MessageSid { get; set; }

    /// <summary>
    /// The status of the message.
    /// </summary>
    public string Status { get; set; }

    /// <summary>
    /// Whether the message was sent successfully.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Any error message returned by Twilio.
    /// </summary>
    public string Error { get; set; }
}
