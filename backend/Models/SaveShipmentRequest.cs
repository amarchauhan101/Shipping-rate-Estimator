namespace ShippingEstimator.Models;


public class SaveShipmentRequest
{
    public ParcelRequest Parcel { get; set; } = new ParcelRequest();
    public string CarrierName { get; set; } = "";
}
