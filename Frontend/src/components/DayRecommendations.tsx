import React, { useState } from "react";
import { Client, WeatherAIRequest } from "../api/generated-weather-data-api-client";
import { useWeather } from "../context/WeatherContext";
import { useThrottle } from "../context/ThrottleService";

const DayRecommendations: React.FC = () => {
  const { weatherDescription, country, city } = useWeather();
  const [dayRecommendations, setDayRecommendations] = useState<string | null>(null);
  const { checkRateLimit, setRateLimit } = useThrottle();

  const handleFetch = async () => {
    if (!checkRateLimit()) return;

    if (!weatherDescription || !city || !country) {
      console.error("Description, city, and country are required.");
      return;
    }

    try {
      const client = new Client();
      const request = new WeatherAIRequest({ description: weatherDescription, city, country });
      const response = await client.getDayRecommendations(request);
      setDayRecommendations(response.result);
      setRateLimit();
    } catch (error) {
      console.error(error);
    }
  };

  return (
    <div>
      <button onClick={handleFetch}>
        Get Day Recommendations
      </button>
      {dayRecommendations && <p>{dayRecommendations}</p>}
    </div>
  );
};

export default DayRecommendations;