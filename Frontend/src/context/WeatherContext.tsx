import React, { createContext, useContext, useState, ReactNode } from "react";

export interface AIResponses {
  whatToWear: string | null;
  dayRecommendations: string | null;
}

interface WeatherContextType {
  country: string;
  city: string;
  weatherDescription: string;
  aiResponses: AIResponses;
  setWeatherData: (country: string, city: string, description: string) => void;
  setWhatToWear: (response: string) => void;
  setDayRecommendations: (response: string) => void;
  clearAIResponses: () => void;
}

const WeatherContext = createContext<WeatherContextType | undefined>(undefined);

export const WeatherProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
  const [country, setCountry] = useState<string>("");
  const [city, setCity] = useState<string>("");
  const [weatherDescription, setWeatherDescription] = useState<string>("");
  const [aiResponses, setAIResponses] = useState<AIResponses>({
    whatToWear: null,
    dayRecommendations: null,
  });

  const setWeatherData = (country: string, city: string, description: string) => {
    setCountry(country);
    setCity(city);
    setWeatherDescription(description);
  };

  const setWhatToWear = (response: string) => {
    setAIResponses((prev) => ({ ...prev, whatToWear: response }));
  };

  const setDayRecommendations = (response: string) => {
    setAIResponses((prev) => ({ ...prev, dayRecommendations: response }));
  };

  const clearAIResponses = () => {
    setAIResponses({ whatToWear: null, dayRecommendations: null });
  };

  return (
    <WeatherContext.Provider value={{ country, city, weatherDescription, aiResponses, setWeatherData, setWhatToWear, setDayRecommendations, clearAIResponses }}>
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