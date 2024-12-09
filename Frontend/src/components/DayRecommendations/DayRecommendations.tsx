import React from "react";
import { useWeather } from "../../context/WeatherContext";
import useFetchAI from "../../hooks/useFetchAI/useFetchAI";

const DayRecommendations: React.FC = () => {
  const { aiResponses, setDayRecommendations } = useWeather();
  const { result: dayRecommendations, loading, fetchData } = useFetchAI((client, request) => client.getDayRecommendations(request), setDayRecommendations);

  return (
    <div>
      <button onClick={fetchData} disabled={loading}>
        {loading ? "Loading..." : "Get Day Recommendations"}
      </button>
      {aiResponses.dayRecommendations && <p>{aiResponses.dayRecommendations}</p>}
    </div>
  );
};

export default DayRecommendations;