import { WeatherAIRequest, WeatherDescriptionRequest } from "./generated-weather-data-api-client";

export const createWeatherAIRequest = (description: string, city: string, country: string): WeatherAIRequest => {
  if (!description || !city || !country) {
    throw new Error("Description, city, and country are required.");
  }
  return new WeatherAIRequest({ description, city, country });
};

export const createWeatherDescriptionRequest = (city: string, countryCode: string): WeatherDescriptionRequest => {
  if (!city || !countryCode) {
    throw new Error("City and countryCode are required.");
  }
  return new WeatherDescriptionRequest({ city, countryCode });
};