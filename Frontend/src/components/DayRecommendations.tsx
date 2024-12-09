import React, { useState } from "react";
import {
  Client,
  WeatherAIRequest,
} from "../api/generated-weather-data-api-client";
import { useWeather } from "../context/WeatherContext";
import { handleApiError } from "../api/errorHandlerUtils";

const DayRecommendations: React.FC = () => {
  const { weatherDescription, country, city } = useWeather();
  const [dayRecommendations, setDayRecommendations] = useState<string | null>(
    null
  );

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

      const response = await client.getDayRecommendations(request);

      if (response.result && response.result.content) {
        setDayRecommendations(response.result.content);
      }
    } catch (error) {
      handleApiError(error);
    }
  };

  return (
    <div>
      <button onClick={handleFetch}>Get Day Recommendations</button>
      {dayRecommendations && <p>{dayRecommendations}</p>}
    </div>
  );
};

export default DayRecommendations;
