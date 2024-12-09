import { render, screen } from '@testing-library/react';
import '@testing-library/jest-dom';
import { WeatherProvider } from './context/WeatherContext';
import App from './App';

describe('App', () => {
  test('renders App component', () => {
    render(
      <WeatherProvider>
        <App />
      </WeatherProvider>
    );

    expect(screen.getByText('Weather Forecast')).toBeInTheDocument();
    expect(screen.getByTestId('city-id')).toBeInTheDocument();
    expect(screen.getByTestId('country-id')).toBeInTheDocument();
  });
});