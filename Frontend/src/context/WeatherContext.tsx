import React, { createContext, useContext, useState, ReactNode } from "react";

interface WeatherContextType {
  country: string;
  city: string;
  weatherDescription: string;
  setWeatherData: (country: string, city: string, description: string) => void;
}

const WeatherContext = createContext<WeatherContextType | undefined>(undefined);

export const WeatherProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
  const [country, setCountry] = useState<string>("");
  const [city, setCity] = useState<string>("");
  const [weatherDescription, setWeatherDescription] = useState<string>("");

  const setWeatherData = (country: string, city: string, description: string) => {
    setCountry(country);
    setCity(city);
    setWeatherDescription(description);
  };

  return (
    <WeatherContext.Provider value={{ country, city, weatherDescription, setWeatherData }}>
      {children}
    </WeatherContext.Provider>
  );
};

export const useWeather = (): WeatherContextType => {
  const context = useContext(WeatherContext);
  if (!context) {
    throw new Error("useWeather must be used within a WeatherProvider");
  }
  return context;
};