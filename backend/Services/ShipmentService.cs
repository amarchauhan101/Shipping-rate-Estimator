using ShippingEstimator.Models;

namespace ShippingEstimator.Services;

public class ShipmentService
{
    private readonly QuoteService _quoteService;
    private readonly List<Shipment> _shipments = new List<Shipment>();
    private int _nextId = 1;

    public ShipmentService(QuoteService quoteService)
    {
        _quoteService = quoteService;
    }

    public List<Shipment> GetAll()
    {
        return _shipments;
    }

    public Shipment Save(ParcelRequest parcel, string carrierName)
    {
  
        QuoteResult quote = _quoteService.GetQuote(carrierName, parcel);
        if (!quote.IsAvailable)
        {
            throw new Exception(quote.DeclineReason);
        }

        Shipment shipment = new Shipment();
        shipment.Id = _nextId;
        shipment.DestinationName = parcel.DestinationName;
        shipment.WeightKg = parcel.WeightKg;
        shipment.CarrierName = quote.CarrierName;
        shipment.Price = quote.Price;
        shipment.Status = "Quoted";

        _nextId++;
        _shipments.Add(shipment);
        return shipment;
    }

    public Shipment ChangeStatus(int id, string newStatus)
    {
        Shipment? shipment = _shipments.Find(s => s.Id == id);
        if (shipment == null)
        {
            throw new Exception("Shipment " + id + " not found");
        }

        bool allowed =
            (shipment.Status == "Quoted" && (newStatus == "Booked" || newStatus == "Cancelled")) ||
            (shipment.Status == "Booked" && (newStatus == "Dispatched" || newStatus == "Cancelled")) ||
            (shipment.Status == "Dispatched" && newStatus == "Delivered");

        if (!allowed)
        {
            throw new Exception("Cannot change status from " + shipment.Status + " to " + newStatus);
        }

        shipment.Status = newStatus;
        return shipment;
    }
}
