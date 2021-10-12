import "./App.css";
import React, { useState } from "react";

function App() {
  const [display, setDisplay] = useState([]);
  const [city, setCity] = useState([]);
  const [countryCode, setCountryCode] = useState([]);

  function fetchRequest() {
    fetch(
      `https://localhost:44396/weatherforecast/forecast?city=${city}&countryCode=${countryCode}`,
      {
        headers: {
          ClientId: "Bob",
          "content-type": "application/json",
        },
      }
    )
      .then(function (response) {
        if (response.status === 200) {
          return response.json();
        }
        if (response.status === 400) {
          return "400 - Bad Request";
        }
        if (response.status === 401) {
          return "Unauthorised.";
        }
        if (response.status === 429) {
          return "Rate Limit Reached";
        }
        if (response.status === 500) {
          return "Bad Request - 500 Error";
        }
        return "An error has occured";
      })
      .then((response) => {
        setDisplay(response);
      })
      .catch(() => setDisplay("An error has occured."));
  }

  return (
    <div className="App">
      <h1>Weather Forecast</h1>
      <label>
        Country:
        <input
          data-testid="city-id"
          type="text"
          value={countryCode}
          onChange={(e) => setCountryCode(e.target.value)}
        />
      </label>
      <br />
      <label>
        City:
        <input
          data-testid="country-id"
          type="text"
          value={city}
          onChange={(e) => setCity(e.target.value)}
        />
      </label>
      <br />
      <button className="button" onClick={fetchRequest}>
        Submit
      </button>
      <div>
        <h2>{display}</h2>
      </div>
    </div>
  );
}

export default App;
