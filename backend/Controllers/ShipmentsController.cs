using Microsoft.AspNetCore.Mvc;
using ShippingEstimator.Models;
using ShippingEstimator.Services;

namespace ShippingEstimator.Controllers;

[ApiController]
[Route("api/shipments")]
public class ShipmentsController : ControllerBase
{
    private readonly ShipmentService _shipmentService;

    public ShipmentsController(ShipmentService shipmentService)
    {
        _shipmentService = shipmentService;
    }

    // GET /api/shipments
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_shipmentService.GetAll());
    }

    // POST /api/shipments
    [HttpPost]
    public IActionResult Save([FromBody] SaveShipmentRequest request)
    {
        try
        {
            return Ok(_shipmentService.Save(request.Parcel, request.CarrierName));
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

   
    [HttpPost("{id}/status/{newStatus}")]
    public IActionResult ChangeStatus(int id, string newStatus)
    {
        try
        {
            return Ok(_shipmentService.ChangeStatus(id, newStatus));
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
