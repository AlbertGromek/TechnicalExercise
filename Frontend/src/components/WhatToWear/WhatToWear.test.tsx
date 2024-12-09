import React from "react";
import { render, screen, fireEvent } from "@testing-library/react";
import "@testing-library/jest-dom/extend-expect";
import { useWeather } from "../../context/WeatherContext";
import WhatToWear from "./WhatToWear";
import useFetchAI from "../../hooks/useFetchAI/useFetchAI";

jest.mock("../../context/WeatherContext", () => ({
  useWeather: jest.fn(),
}));

jest.mock("../../hooks/useFetchAI/useFetchAI", () => jest.fn());

describe("WhatToWear", () => {
  const mockSetWhatToWear = jest.fn();
  const mockFetchData = jest.fn();

  beforeEach(() => {
    jest.clearAllMocks();
  });

  it("renders the component", () => {
    (useFetchAI as jest.Mock).mockReturnValue({
      result: null,
      loading: false,
      fetchData: mockFetchData,
    });

    (useWeather as jest.Mock).mockReturnValue({
      aiResponses: { whatToWear: null, dayRecommendations: null },
      setWhatToWear: mockSetWhatToWear,
    });

    render(<WhatToWear />);

    expect(screen.queryByText("Get What to Wear")).toBeInTheDocument();
  });

  it("displays loading state when fetching data", () => {
    (useFetchAI as jest.Mock).mockReturnValue({
      result: null,
      loading: true,
      fetchData: mockFetchData,
    });

    (useWeather as jest.Mock).mockReturnValue({
      aiResponses: { whatToWear: null, dayRecommendations: null },
      setWhatToWear: mockSetWhatToWear,
    });

    render(<WhatToWear />);

    expect(screen.queryByText("Loading...")).toBeInTheDocument();
    expect(screen.queryByRole("button")).toBeDisabled();
  });

  it("displays the fetched AI response", async () => {
    const mockResponse = "Wear sunglasses";

    (useFetchAI as jest.Mock).mockReturnValue({
      result: mockResponse,
      loading: false,
      fetchData: mockFetchData,
    });

    (useWeather as jest.Mock).mockReturnValue({
      aiResponses: { whatToWear: mockResponse, dayRecommendations: null },
      setWhatToWear: mockSetWhatToWear,
    });

    render(<WhatToWear />);

    expect(screen.queryByText("Get What to Wear")).toBeInTheDocument();
    expect(screen.queryByText(mockResponse)).toBeInTheDocument();
  });

  it("calls fetchData when button is clicked", () => {
    (useFetchAI as jest.Mock).mockReturnValue({
      result: null,
      loading: false,
      fetchData: mockFetchData,
    });

    (useWeather as jest.Mock).mockReturnValue({
      aiResponses: { whatToWear: null, dayRecommendations: null },
      setWhatToWear: mockSetWhatToWear,
    });

    render(<WhatToWear />);

    const button = screen.queryByText("Get What to Wear");
    expect(button).toBeInTheDocument();

    fireEvent.click(button!);

    expect(mockFetchData).toHaveBeenCalled();
  });
});