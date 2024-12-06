import { useState } from "react";
import { weatherApiClient } from "../api";

interface FetchWeatherDataProps {
  city: string;
  countryCode: string;
}

const useWeatherData = () => {
  const [weatherReport, setWeatherReport] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState<boolean>(false);

  const fetchWeatherData = async ({ city, countryCode }: FetchWeatherDataProps) => {
    setLoading(true);
    setError(null);

    try {
      const weather = await weatherApiClient.getWeatherForecastDescription(city, countryCode);
      setWeatherReport(weather);
    } catch (err: any) {
      // Leaving these here to make it easier for anyone who is testing this non production code :D 
      if (err.status === 400) {
        setError("400 - Bad Request");
      } else if (err.status === 401) {
        setError("Unauthorized.");
      } else if (err.status === 429) {
        setError("Rate Limit Reached");
      } else if (err.status === 500) {
        setError("Server Error - 500");
      } else {
        setError("An unexpected error occurred.");
      }
    } finally {
      setLoading(false);
    }
  };

  return { weatherReport, error, loading, fetchWeatherData };
};

export default useWeatherData;
