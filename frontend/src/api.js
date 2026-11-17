

async function callApi(url, method, body) {
  const options = { method: method, headers: { "Content-Type": "application/json" } };
  if (body) {
    options.body = JSON.stringify(body);
  }

  const response = await fetch(url, options);
  const data = await response.json().catch(() => null);

  if (!response.ok) {
    throw new Error((data && (data.message || data.title)) || "Something went wrong");
  }
  return data;
}

export function getDestinations() {
  return callApi("/api/destinations", "GET");
}

export function getQuotes(parcel) {
  return callApi("/api/quotes", "POST", parcel);
}

export function saveShipment(parcel, carrierName) {
  return callApi("/api/shipments", "POST", { parcel: parcel, carrierName: carrierName });
}

export function getShipments() {
  return callApi("/api/shipments", "GET");
}

export function changeStatus(id, newStatus) {
  return callApi("/api/shipments/" + id + "/status/" + newStatus, "POST");
}
