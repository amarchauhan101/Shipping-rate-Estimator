using Microsoft.AspNetCore.Mvc;
using ShippingEstimator.Models;
using ShippingEstimator.Services;

namespace ShippingEstimator.Controllers;

[ApiController]
[Route("api")]
public class QuotesController : ControllerBase
{
    private readonly QuoteService _quoteService;

    public QuotesController(QuoteService quoteService)
    {
        _quoteService = quoteService;
    }

    [HttpGet("destinations")]
    public IActionResult GetDestinations()
    {
        return Ok(_quoteService.GetDestinations());
    }


    [HttpPost("quotes")]
    public IActionResult GetQuotes([FromBody] ParcelRequest parcel)
    {
        if (parcel.WeightKg <= 0 || parcel.LengthCm <= 0 || parcel.WidthCm <= 0 || parcel.HeightCm <= 0)
        {
            return BadRequest(new { message = "Weight, length, width and height must be more than 0" });
        }
        if (parcel.DeclaredValue < 0)
        {
            return BadRequest(new { message = "Declared value cannot be negative" });
        }

        try
        {
            return Ok(_quoteService.GetAllQuotes(parcel));
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
