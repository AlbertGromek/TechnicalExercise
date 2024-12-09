import "./App.css";
import React from "react";
import { WeatherProvider, useWeather } from "./context/WeatherContext";
import WeatherDescription from "./components/WeatherDescription/WeatherDescription";
import WhatToWear from "./components/WhatToWear/WhatToWear";
import DayRecommendations from "./components/DayRecommendations/DayRecommendations";

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