import React from "react";
import { useWeather } from "../context/WeatherContext";
import useFetchAI from "../hooks/useFetchAI/useFetchAI";

const WhatToWear: React.FC = () => {
  const { aiResponses, setWhatToWear } = useWeather();
  const { result: whatToWear, loading, fetchData } = useFetchAI((client, request) => client.getWhatToWear(request), setWhatToWear);

  return (
    <div>
      <button onClick={fetchData} disabled={loading}>
        {loading ? "Loading..." : "Get What to Wear"}
      </button>
      {aiResponses.whatToWear && <p>{aiResponses.whatToWear}</p>}
    </div>
  );
};

export default WhatToWear;