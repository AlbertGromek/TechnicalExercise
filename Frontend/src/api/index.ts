import { Client } from "./generated-weather-data-api-client";

const apiBaseUrl = process.env.REACT_APP_API_BASE_URL || "http://localhost:7278/api";

export const weatherApiClient = new Client(apiBaseUrl);