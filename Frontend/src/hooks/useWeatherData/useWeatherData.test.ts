import useWeatherData from './useWeatherData';

jest.mock('../api');

describe('useWeatherData', () => {
  it('should fetch weather data successfully', async () => {
    const mockWeather = 'Sunny';
    (weatherApiClient.getWeatherForecastDescription as jest.Mock).mockResolvedValue(mockWeather);

    const { result, waitForNextUpdate } = renderHook(() => useWeatherData());

    act(() => {
      result.current.fetchWeatherData({ city: 'London', countryCode: 'GB' });
    });

    await waitForNextUpdate();

    expect(result.current.weatherReport).toBe(mockWeather);
    expect(result.current.error).toBeNull();
    expect(result.current.loading).toBe(false);
  });

  it('should handle errors correctly', async () => {
    const mockError = { status: 400 };
    (weatherApiClient.getWeatherForecastDescription as jest.Mock).mockRejectedValue(mockError);

    const { result, waitForNextUpdate } = renderHook(() => useWeatherData());

    act(() => {
      result.current.fetchWeatherData({ city: 'InvalidCity', countryCode: 'XX' });
    });

    await waitForNextUpdate();

    expect(result.current.weatherReport).toBeNull();
    expect(result.current.error).toBe('400 - Bad Request');
    expect(result.current.loading).toBe(false);
  });
});