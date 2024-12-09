import React, { useState } from "react";
import { Client, WeatherAIRequest } from "../api/generated-weather-data-api-client";
import { useWeather } from "../context/WeatherContext";
import { handleApiError } from "../api/errorHandlerUtils";

const WhatToWear: React.FC = () => {
  const { weatherDescription, country, city } = useWeather();
  const [whatToWear, setWhatToWear] = useState<string | null>(null);

  const handleFetch = async () => {
    if (!weatherDescription || !city || !country) {
      console.error("Description, city, and country are required.");
      return;
    }

    try {
      const client = new Client();
      const request = new WeatherAIRequest({
        description: weatherDescription,
        city,
        country,
      });

      const response = await client.getWhatToWear(request);

      if (response.result && response.result.content) {
        setWhatToWear(response.result.content);
      }
    } catch (error) {
      handleApiError(error); 
    }
  };

  return (
    <div>
      <button onClick={handleFetch}>Get What to Wear</button>
      {whatToWear && <p>{whatToWear}</p>}
    </div>
  );
};

export default WhatToWear;
