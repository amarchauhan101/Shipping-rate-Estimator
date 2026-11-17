namespace ShippingEstimator.Models;


public class ParcelRequest
{
    public decimal WeightKg { get; set; }
    public decimal LengthCm { get; set; }
    public decimal WidthCm { get; set; }
    public decimal HeightCm { get; set; }
    public string DestinationName { get; set; } = "";
    public decimal DeclaredValue { get; set; }

    public bool WantsSignature { get; set; }
    public bool WantsInsurance { get; set; }
    public bool WantsExpress { get; set; }
    public bool WantsFragile { get; set; }
}
