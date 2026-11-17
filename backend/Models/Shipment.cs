namespace ShippingEstimator.Models;

public class Shipment
{
    public int Id { get; set; }
    public string DestinationName { get; set; } = "";
    public decimal WeightKg { get; set; }
    public string CarrierName { get; set; } = "";
    public decimal Price { get; set; }


    public string Status { get; set; } = "Quoted";
}
