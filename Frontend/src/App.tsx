import "./App.css";
import React, { useState } from "react";
import useWeatherData from "./hooks/useWeatherData";

const App: React.FC = () => {
  const [city, setCity] = useState<string>("");
  const [countryCode, setCountryCode] = useState<string>("");
  const { weatherReport, error, loading, fetchWeatherData } = useWeatherData();

  const handleFetch = () => {
    fetchWeatherData({ city, countryCode });
  };

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
      <button className="button" onClick={handleFetch} disabled={loading}>
        Submit
      </button>
      <div>
        {loading && <p>Loading...</p>}
        {error && <p>{error}</p>}
        {weatherReport && <h2>{weatherReport}</h2>}
      </div>
    </div>
  );
};

export default App;