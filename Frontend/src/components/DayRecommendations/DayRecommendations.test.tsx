import { render, screen, fireEvent } from "@testing-library/react";
import "@testing-library/jest-dom/extend-expect";
import { useWeather } from "../../context/WeatherContext";
import DayRecommendations from "./DayRecommendations";
import useFetchAI from "../../hooks/useFetchAI/useFetchAI";

jest.mock("../../context/WeatherContext", () => ({
  useWeather: jest.fn(),
}));

jest.mock("../../hooks/useFetchAI/useFetchAI", () => jest.fn());

describe("DayRecommendations", () => {
  const mockSetDayRecommendations = jest.fn();
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
      setDayRecommendations: mockSetDayRecommendations,
    });

    render(<DayRecommendations />);

    expect(screen.queryByText("Get Day Recommendations")).toBeInTheDocument();
  });

  it("displays loading state when fetching data", () => {
    (useFetchAI as jest.Mock).mockReturnValue({
      result: null,
      loading: true,
      fetchData: mockFetchData,
    });

    (useWeather as jest.Mock).mockReturnValue({
      aiResponses: { whatToWear: null, dayRecommendations: null },
      setDayRecommendations: mockSetDayRecommendations,
    });

    render(<DayRecommendations />);

    expect(screen.queryByText("Loading...")).toBeInTheDocument();
    expect(screen.queryByRole("button")).toBeDisabled();
  });

  it("displays the fetched AI response", async () => {
    const mockResponse = "Go for a walk";

    (useFetchAI as jest.Mock).mockReturnValue({
      result: mockResponse,
      loading: false,
      fetchData: mockFetchData,
    });

    (useWeather as jest.Mock).mockReturnValue({
      aiResponses: { whatToWear: null, dayRecommendations: mockResponse },
      setDayRecommendations: mockSetDayRecommendations,
    });

    render(<DayRecommendations />);

    expect(screen.queryByText("Get Day Recommendations")).toBeInTheDocument();
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
      setDayRecommendations: mockSetDayRecommendations,
    });

    render(<DayRecommendations />);

    const button = screen.queryByText("Get Day Recommendations");
    expect(button).toBeInTheDocument();

    fireEvent.click(button!);

    expect(mockFetchData).toHaveBeenCalled();
  });
});