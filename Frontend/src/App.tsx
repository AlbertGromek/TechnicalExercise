import "./App.css";
import React from "react";
import { WeatherProvider, useWeather } from "./context/WeatherContext";
import WeatherDescription from "./components/WeatherDescription";
import WhatToWear from "./components/WhatToWear";
import DayRecommendations from "./components/DayRecommendations";

const App: React.FC = () => {
  const { weatherDescription } = useWeather();

  return (
      <div className="App">
        <WeatherDescription />
        {weatherDescription && (
          <>
            <WhatToWear />
            <DayRecommendations />
          </>
        )}
      </div>
  );
};

export default App;