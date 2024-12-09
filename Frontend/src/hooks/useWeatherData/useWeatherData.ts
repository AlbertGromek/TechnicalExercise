import { useState } from "react";
import {
  Client,
  WeatherDescriptionRequest,
} from "../../api/generated-weather-data-api-client";
import { useWeather } from "../../context/WeatherContext";

const useWeatherData = () => {
  const [weatherReport, setWeatherReport] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState<boolean>(false);
  const { setWeatherData } = useWeather();

  const fetchWeatherData = async (request: WeatherDescriptionRequest) => {
    setLoading(true);
    setError(null);

    const client = new Client();

    try {
      const weather = await client.getWeatherForecastDescription(request);
      setWeatherReport(weather.result);
      if (request.countryCode && request.city && weather) {
        setWeatherData(request.countryCode, request.city, weather.result);
      } else {
        throw new Error("Invalid weather data");
      }
    } catch (err: any) {
      if (err.status === 400) {
        setError("400 - Bad Request");
      } else if (err.status === 401) {
        setError("Unauthorized.");
      } else if (err.status === 429) {
        setError("Rate Limit Reached");
      } else if (err.status === 500) {
        setError("Server Error - 500");
      } else if (err.status === 404) {
        setError("City or country not found.");
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
