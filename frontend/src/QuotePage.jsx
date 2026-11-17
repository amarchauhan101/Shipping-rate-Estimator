import { useEffect, useState } from "react";
import { getDestinations, getQuotes, saveShipment } from "./api.js";

export default function QuotePage({ onSaved }) {
  const [destinations, setDestinations] = useState([]);
  const [quotes, setQuotes] = useState([]);
  const [message, setMessage] = useState("");

 
  const [weightKg, setWeightKg] = useState("");
  const [lengthCm, setLengthCm] = useState("");
  const [widthCm, setWidthCm] = useState("");
  const [heightCm, setHeightCm] = useState("");
  const [destinationName, setDestinationName] = useState("");
  const [declaredValue, setDeclaredValue] = useState("0");
  const [wantsSignature, setWantsSignature] = useState(false);
  const [wantsInsurance, setWantsInsurance] = useState(false);
  const [wantsExpress, setWantsExpress] = useState(false);
  const [wantsFragile, setWantsFragile] = useState(false);


  useEffect(() => {
    getDestinations()
      .then((list) => setDestinations(list))
      .catch((error) => setMessage(error.message));
  }, []);

  function buildParcel() {
    return {
      weightKg: Number(weightKg),
      lengthCm: Number(lengthCm),
      widthCm: Number(widthCm),
      heightCm: Number(heightCm),
      destinationName: destinationName,
      declaredValue: Number(declaredValue),
      wantsSignature: wantsSignature,
      wantsInsurance: wantsInsurance,
      wantsExpress: wantsExpress,
      wantsFragile: wantsFragile,
    };
  }

  async function handleSubmit(event) {
    event.preventDefault();
    setMessage("");
    try {
      setQuotes(await getQuotes(buildParcel()));
    } catch (error) {
      setQuotes([]);
      setMessage(error.message);
    }
  }

  async function handleSave(carrierName) {
    setMessage("");
    try {
      await saveShipment(buildParcel(), carrierName);
      onSaved();
    } catch (error) {
      setMessage(error.message);
    }
  }

  return (
    <div className="two-columns">
      <form onSubmit={handleSubmit}>
        <h2>Parcel details</h2>

        <label>
          Weight (kg)
          <input type="number" min="0.01" step="0.01" required
            value={weightKg} onChange={(e) => setWeightKg(e.target.value)} />
        </label>

        <label>
          Length (cm)
          <input type="number" min="1" step="0.1" required
            value={lengthCm} onChange={(e) => setLengthCm(e.target.value)} />
        </label>

        <label>
          Width (cm)
          <input type="number" min="1" step="0.1" required
            value={widthCm} onChange={(e) => setWidthCm(e.target.value)} />
        </label>

        <label>
          Height (cm)
          <input type="number" min="1" step="0.1" required
            value={heightCm} onChange={(e) => setHeightCm(e.target.value)} />
        </label>

        <label>
          Destination
          <select required value={destinationName} onChange={(e) => setDestinationName(e.target.value)}>
            <option value="">Choose a destination</option>
            {destinations.map((place) => (
              <option key={place.name} value={place.name}>{place.name}</option>
            ))}
          </select>
        </label>

        <label>
          Declared value (₹)
          <input type="number" min="0" step="1" required
            value={declaredValue} onChange={(e) => setDeclaredValue(e.target.value)} />
        </label>

        <label className="checkbox">
          <input type="checkbox" checked={wantsSignature} onChange={(e) => setWantsSignature(e.target.checked)} />
          Signature on delivery
        </label>
        <label className="checkbox">
          <input type="checkbox" checked={wantsInsurance} onChange={(e) => setWantsInsurance(e.target.checked)} />
          Insurance
        </label>
        <label className="checkbox">
          <input type="checkbox" checked={wantsExpress} onChange={(e) => setWantsExpress(e.target.checked)} />
          Express
        </label>
        <label className="checkbox">
          <input type="checkbox" checked={wantsFragile} onChange={(e) => setWantsFragile(e.target.checked)} />
          Fragile handling
        </label>

        <button type="submit" className="primary">Get quotes</button>
      </form>

      <div>
        <h2>Quotes</h2>

        {message && <p className="error">{message}</p>}

        {quotes.length === 0 && !message && (
          <p className="hint">Fill in the parcel details and click "Get quotes".</p>
        )}

        {quotes.map((quote) => (
          <div className="card" key={quote.carrierName}>
            <div>
              <strong>{quote.carrierName}</strong>
              {quote.isAvailable ? (
                <div className="hint">Billable weight: {quote.billableWeightKg} kg</div>
              ) : (
                <div className="error-text">{quote.declineReason}</div>
              )}
            </div>

            {quote.isAvailable && (
              <div className="right">
                <div className="price">₹{quote.price.toFixed(2)}</div>
                <button onClick={() => handleSave(quote.carrierName)}>Save this quote</button>
              </div>
            )}
          </div>
        ))}
      </div>
    </div>
  );
}
