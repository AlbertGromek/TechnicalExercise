import React, { useState } from "react";
import {
  Client,
  SwaggerException,
  WeatherAIRequest,
} from "../api/generated-weather-data-api-client";
import { useWeather } from "../context/WeatherContext";
import { useThrottle } from "../context/ThrottleService";

const WhatToWear: React.FC = () => {
  const { weatherDescription, country, city } = useWeather();
  const [whatToWear, setWhatToWear] = useState<string | null>(null);
  const { checkRateLimit, setRateLimit } = useThrottle();

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
    } catch (error: any) {
      // Handle rate-limiting error
      if (error.status === 429) {
        const retryAfter = error.headers?.["retry-after"];
        const retryAfterSeconds = retryAfter ? parseInt(retryAfter, 10) : null;

        if (retryAfterSeconds) {
          const retryTime = new Date(Date.now() + retryAfterSeconds * 1000);
          alert(
            `Rate limit exceeded. Try again in ${retryAfterSeconds} seconds (at ${retryTime.toLocaleTimeString()}).`
          );
        } else {
          alert("Rate limit exceeded. Please retry later.");
        }
        return;
      }

      // Handle other error responses with content
      if (error.content) {
        alert(error.content);
      } else {
        alert("An unexpected error occurred. Please try again.");
      }
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
