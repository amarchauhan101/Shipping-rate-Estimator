import { useEffect, useState } from "react";
import { getShipments, changeStatus } from "./api.js";


const nextSteps = {
  Quoted: [
    { status: "Booked", label: "Book" },
    { status: "Cancelled", label: "Cancel" },
  ],
  Booked: [
    { status: "Dispatched", label: "Mark dispatched" },
    { status: "Cancelled", label: "Cancel" },
  ],
  Dispatched: [{ status: "Delivered", label: "Mark delivered" }],
  Delivered: [],
  Cancelled: [],
};

export default function ShipmentsPage() {
  const [shipments, setShipments] = useState([]);
  const [message, setMessage] = useState("");

  useEffect(() => {
    loadShipments();
  }, []);

  async function loadShipments() {
    try {
      setShipments(await getShipments());
    } catch (error) {
      setMessage(error.message);
    }
  }

  async function handleChange(id, newStatus) {
    setMessage("");
    try {
      await changeStatus(id, newStatus);
    } catch (error) {
      setMessage(error.message);
    }
    loadShipments();
  }

  return (
    <div>
      <h2>Shipments</h2>

      {message && <p className="error">{message}</p>}

      {shipments.length === 0 && (
        <p className="hint">No shipments yet. Get a quote and save one.</p>
      )}

      {shipments.map((shipment) => (
        <div className="card" key={shipment.id}>
          <div>
            <strong>#{shipment.id} to {shipment.destinationName}</strong>{" "}
            <span className={"badge " + shipment.status}>{shipment.status}</span>
            <div className="hint">
              {shipment.carrierName}, {shipment.weightKg} kg, ₹{shipment.price.toFixed(2)}
            </div>
          </div>

          <div className="right">
            {nextSteps[shipment.status].map((step) => (
              <button key={step.status} onClick={() => handleChange(shipment.id, step.status)}>
                {step.label}
              </button>
            ))}
          </div>
        </div>
      ))}
    </div>
  );
}
