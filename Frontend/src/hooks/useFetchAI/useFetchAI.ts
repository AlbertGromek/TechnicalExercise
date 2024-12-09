import { useState } from "react";
import { handleApiError } from "../../api/errorHandlerUtils";
import { Client, WeatherAIRequest } from "../../api/generated-weather-data-api-client";
import { useWeather } from "../../context/WeatherContext";

const useFetchAI = (fetchFunction: (client: Client, request: WeatherAIRequest) => Promise<any>, setResponse: (response: string) => void) => {
  const { weatherDescription, country, city } = useWeather();
  const [result, setResult] = useState<string | null>(null);
  const [loading, setLoading] = useState<boolean>(false);

  const fetchData = async () => {
    if (!weatherDescription || !city || !country) {
      console.error("Description, city, and country are required.");
      return;
    }

    setLoading(true);

    try {
      const client = new Client();
      const request = new WeatherAIRequest({
        description: weatherDescription,
        city,
        country,
      });

      const response = await fetchFunction(client, request);

      if (response.result && response.result.content) {
        setResult(response.result.content);
        setResponse(response.result.content);
      }
    } catch (error) {
      handleApiError(error);
    } finally {
      setLoading(false);
    }
  };

  return { result, loading, fetchData };
};

export default useFetchAI;