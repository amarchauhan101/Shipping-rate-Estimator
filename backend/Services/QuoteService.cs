using ShippingEstimator.Models;

namespace ShippingEstimator.Services;

public class QuoteService
{
    private readonly List<Destination> _destinations = new List<Destination>
    {
        new Destination { Name = "Nagpur", Zone = 1, IsRemote = false },
        new Destination { Name = "Mumbai", Zone = 2, IsRemote = false },
        new Destination { Name = "Delhi", Zone = 3, IsRemote = false },
        new Destination { Name = "Leh Ladakh", Zone = 3, IsRemote = true },
        new Destination { Name = "Singapore", Zone = 4, IsRemote = false },
        new Destination { Name = "London", Zone = 4, IsRemote = false },
    };

    public List<Destination> GetDestinations()
    {
        return _destinations;
    }

  
    public List<QuoteResult> GetAllQuotes(ParcelRequest parcel)
    {
        Destination destination = FindDestination(parcel.DestinationName);

        List<QuoteResult> quotes = new List<QuoteResult>();
        quotes.Add(CalculateParcelo(parcel, destination));
        quotes.Add(CalculateAirSwift(parcel, destination));
        quotes.Add(CalculateZoneLine(parcel, destination));
        return quotes;
    }

 
    public QuoteResult GetQuote(string carrierName, ParcelRequest parcel)
    {
        foreach (QuoteResult quote in GetAllQuotes(parcel))
        {
            if (quote.CarrierName == carrierName)
            {
                return quote;
            }
        }
        throw new Exception("Unknown carrier: " + carrierName);
    }


    private QuoteResult CalculateParcelo(ParcelRequest parcel, Destination destination)
    {
        string name = "Parcelo Ground";

        if (parcel.WantsExpress)
        {
            return Decline(name, "Does not support express delivery");
        }
        if (destination.Zone > 3)
        {
            return Decline(name, "Delivers only to zones 1 to 3");
        }

        decimal weight = RoundUp(parcel.WeightKg, 0.5m);
        if (weight > 25)
        {
            return Decline(name, "Accepts parcels up to 25 kg");
        }

        decimal price = 50 + 20 * weight;

        if (parcel.WantsSignature)
        {
            price += 40;
        }
        if (parcel.WantsFragile)
        {
            price += 60;
        }
        if (parcel.WantsInsurance && parcel.DeclaredValue > 5000)
        {
            
            price += (parcel.DeclaredValue - 5000) * 1.5m / 100;
        }

        return Accept(name, price, weight);
    }


    private QuoteResult CalculateAirSwift(ParcelRequest parcel, Destination destination)
    {
        string name = "AirSwift";

        if (parcel.WantsFragile)
        {
            return Decline(name, "Does not support fragile handling");
        }
      

        decimal weight = GetBillableWeight(parcel, 5000m, 0.5m);
        if (weight > 40)
        {
            return Decline(name, "Accepts billable weight up to 40 kg");
        }

        decimal ratePerKg = 220;
        if (destination.Zone == 1) ratePerKg = 30;
        if (destination.Zone == 2) ratePerKg = 45;
        if (destination.Zone == 3) ratePerKg = 60;

        decimal baseAmount = weight * ratePerKg;
        if (baseAmount < 90)
        {
            baseAmount = 90;
        }

        decimal price = baseAmount + baseAmount * 10 / 100; 

        if (destination.IsRemote)
        {
            price += 150;
        }
        if (parcel.WantsSignature)
        {
            price += 50;
        }
        if (parcel.WantsExpress)
        {
            price += baseAmount * 40 / 100;
        }
        if (parcel.WantsInsurance && parcel.DeclaredValue > 10000)
        {
            
            price += (parcel.DeclaredValue - 10000) * 1m / 100;
        }

        return Accept(name, price, weight);
    }


    private QuoteResult CalculateZoneLine(ParcelRequest parcel, Destination destination)
    {
        string name = "ZoneLine";

        if (parcel.WantsInsurance)
        {
            return Decline(name, "Does not support insurance");
        }
        if (destination.Zone > 3)
        {
            return Decline(name, "Delivers only to zones 1 to 3");
        }

        decimal weight = GetBillableWeight(parcel, 6000m, 1m);
        if (weight > 30)
        {
            return Decline(name, "Accepts billable weight up to 30 kg");
        }

      
        decimal[] row;
        if (weight <= 1) row = new decimal[] { 60, 80, 110 };
        else if (weight <= 5) row = new decimal[] { 120, 160, 210 };
        else if (weight <= 10) row = new decimal[] { 200, 260, 340 };
        else row = new decimal[] { 380, 480, 620 };

        decimal baseAmount = row[destination.Zone - 1];

        decimal price = baseAmount + baseAmount * 8 / 100; 

        if (destination.IsRemote)
        {
            price += 200;
        }
        if (parcel.WantsSignature)
        {
            price += 30;
        }
        if (parcel.WantsExpress)
        {
            price += baseAmount * 25 / 100;
        }
        if (parcel.WantsFragile)
        {
            price += baseAmount * 15 / 100;
        }

        return Accept(name, price, weight);
    }



    private Destination FindDestination(string name)
    {
        foreach (Destination destination in _destinations)
        {
            if (destination.Name.ToLower() == name.Trim().ToLower())
            {
                return destination;
            }
        }
        throw new Exception("Unknown destination: " + name);
    }

    private decimal RoundUp(decimal weight, decimal step)
    {
        return Math.Ceiling(weight / step) * step;
    }

    private decimal GetBillableWeight(ParcelRequest parcel, decimal divisor, decimal step)
    {
        decimal volumeWeight = parcel.LengthCm * parcel.WidthCm * parcel.HeightCm / divisor;
        decimal weight = parcel.WeightKg;
        if (volumeWeight > weight)
        {
            weight = volumeWeight;
        }
        return RoundUp(weight, step);
    }

    private QuoteResult Accept(string carrierName, decimal price, decimal billableWeight)
    {
        QuoteResult result = new QuoteResult();
        result.CarrierName = carrierName;
        result.IsAvailable = true;
        result.Price = Math.Round(price, 2, MidpointRounding.AwayFromZero);
        result.BillableWeightKg = billableWeight;
        return result;
    }

    private QuoteResult Decline(string carrierName, string reason)
    {
        QuoteResult result = new QuoteResult();
        result.CarrierName = carrierName;
        result.IsAvailable = false;
        result.DeclineReason = reason;
        return result;
    }
}
