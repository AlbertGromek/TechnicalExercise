import React from "react";
import { render, screen, fireEvent } from "@testing-library/react";
import "@testing-library/jest-dom/extend-expect";
import WeatherDescription from "./WeatherDescription";
import { useWeather } from "../../context/WeatherContext";
import useWeatherData from "../../hooks/useWeatherData/useWeatherData";

jest.mock("../../context/WeatherContext", () => ({
  useWeather: jest.fn(),
}));

jest.mock("../../hooks/useWeatherData/useWeatherData", () => jest.fn());

describe("WeatherDescription", () => {
  const mockFetchWeatherData = jest.fn();
  const mockClearAIResponses = jest.fn();

  beforeEach(() => {
    jest.clearAllMocks();
  });

  it("renders the component", () => {
    (useWeatherData as jest.Mock).mockReturnValue({
      weatherReport: null,
      error: null,
      loading: false,
      fetchWeatherData: mockFetchWeatherData,
    });

    (useWeather as jest.Mock).mockReturnValue({
      clearAIResponses: mockClearAIResponses,
    });

    render(<WeatherDescription />);

    expect(screen.queryByText("Weather Forecast")).toBeInTheDocument();
    expect(screen.queryByTestId("city-id")).toBeInTheDocument();
    expect(screen.queryByTestId("country-id")).toBeInTheDocument();
  });

  it("displays loading state when fetching data", () => {
    (useWeatherData as jest.Mock).mockReturnValue({
      weatherReport: null,
      error: null,
      loading: true,
      fetchWeatherData: mockFetchWeatherData,
    });

    (useWeather as jest.Mock).mockReturnValue({
      clearAIResponses: mockClearAIResponses,
    });

    render(<WeatherDescription />);

    expect(screen.queryByText("Loading")).toBeInTheDocument();
    expect(screen.queryByRole("button")).toBeDisabled();
  });

  it("displays the fetched weather report", async () => {
    const mockWeatherReport = "Sunny";

    (useWeatherData as jest.Mock).mockReturnValue({
      weatherReport: mockWeatherReport,
      error: null,
      loading: false,
      fetchWeatherData: mockFetchWeatherData,
    });

    (useWeather as jest.Mock).mockReturnValue({
      clearAIResponses: mockClearAIResponses,
    });

    render(<WeatherDescription />);

    expect(screen.queryByText("Weather Forecast")).toBeInTheDocument();
    expect(screen.queryByText(mockWeatherReport)).toBeInTheDocument();
  });

  it("calls fetchWeatherData when button is clicked", () => {
    (useWeatherData as jest.Mock).mockReturnValue({
      weatherReport: null,
      error: null,
      loading: false,
      fetchWeatherData: mockFetchWeatherData,
    });

    (useWeather as jest.Mock).mockReturnValue({
      clearAIResponses: mockClearAIResponses,
    });

    render(<WeatherDescription />);

    const cityInput = screen.queryByTestId("city-id");
    const countryInput = screen.queryByTestId("country-id");

    fireEvent.change(cityInput!, { target: { value: "melbourne" } });
    fireEvent.change(countryInput!, { target: { value: "AU" } });

    const button = screen.queryByText("Submit");
    expect(button).toBeInTheDocument();

    fireEvent.click(button!);

    expect(mockFetchWeatherData).toHaveBeenCalled();
  });

  it("calls clearAIResponses when city or country is changed", () => {
    (useWeatherData as jest.Mock).mockReturnValue({
      weatherReport: null,
      error: null,
      loading: false,
      fetchWeatherData: mockFetchWeatherData,
    });

    (useWeather as jest.Mock).mockReturnValue({
      clearAIResponses: mockClearAIResponses,
    });

    render(<WeatherDescription />);

    const cityInput = screen.queryByTestId("city-id");
    const countryInput = screen.queryByTestId("country-id");

    fireEvent.change(cityInput!, { target: { value: "new york" } });
    fireEvent.change(countryInput!, { target: { value: "us" } });

    expect(mockClearAIResponses).toHaveBeenCalledTimes(2);
  });
});