import React from 'react';
import { render, screen } from '@testing-library/react';
import '@testing-library/jest-dom';
import App from './App';
 
describe('App', () => {
  test('renders App component', () => {
    render(<App />);
 
    expect(screen.getByText('Weather Forecast')).toBeInTheDocument();
    expect(screen.getByTestId('city-id')).toBeInTheDocument();
    expect(screen.getByTestId('country-id')).toBeInTheDocument();

    (screen.getByTestId('city-id'),'melbourne')
  });
});