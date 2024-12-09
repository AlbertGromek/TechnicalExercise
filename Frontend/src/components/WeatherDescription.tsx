import React, { useState } from "react";
import { WeatherDescriptionRequest } from "../api/generated-weather-data-api-client";
import useWeatherData from "../hooks/useWeatherData/useWeatherData";
import { useWeather } from "../context/WeatherContext";

const WeatherDescription: React.FC = () => {
  const [city, setCity] = useState<string>("");
  const [countryCode, setCountryCode] = useState<string>("");
  const { weatherReport, error, loading, fetchWeatherData } = useWeatherData();
  const { clearAIResponses } = useWeather();

  const handleFetch = () => {
    if (!city || !countryCode) {
      console.error("City and countryCode are required.");
      return;
    }

    try {
      const request = new WeatherDescriptionRequest({ city, countryCode });
      fetchWeatherData(request);
    } catch (error) {
      console.error(error);
    }
  };

  const handleCityChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setCity(e.target.value);
    clearAIResponses();
  };

  const handleCountryChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setCountryCode(e.target.value);
    clearAIResponses();
  };

  return (
    <div>
      <h1>Weather Forecast</h1>
      <label>
        Country:
        <input
          data-testid="city-id"
          type="text"
          value={countryCode}
          onChange={handleCountryChange}
        />
      </label>
      <br />
      <label>
        City:
        <input
          data-testid="country-id"
          type="text"
          value={city}
          onChange={handleCityChange}
        />
      </label>
      <br />
      <button className="button" onClick={handleFetch} disabled={loading}>
        {loading ? "Loading" : "Submit"}
      </button>
      <div>
        {error && <p>{error}</p>}
        {weatherReport && <h2>{weatherReport}</h2>}
      </div>
    </div>
  );
};

export default WeatherDescription;