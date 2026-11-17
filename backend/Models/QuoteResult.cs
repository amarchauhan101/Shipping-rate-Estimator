namespace ShippingEstimator.Models;

public class QuoteResult
{
    public string CarrierName { get; set; } = "";
    public bool IsAvailable { get; set; }
    public string DeclineReason { get; set; } = "";
    public decimal BillableWeightKg { get; set; }
    public decimal Price { get; set; }
}
