import { useState } from "react";
import QuotePage from "./QuotePage.jsx";
import ShipmentsPage from "./ShipmentsPage.jsx";

export default function App() {
  const [page, setPage] = useState("quote");

  return (
    <div className="container">
      <h1>Shipping Estimator</h1>

      <div className="tabs">
        <button className={page === "quote" ? "active" : ""} onClick={() => setPage("quote")}>
          Get quotes
        </button>
        <button className={page === "shipments" ? "active" : ""} onClick={() => setPage("shipments")}>
          Shipments
        </button>
      </div>

      {page === "quote" && <QuotePage onSaved={() => setPage("shipments")} />}
      {page === "shipments" && <ShipmentsPage />}
    </div>
  );
}
