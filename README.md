# Technical Exercise

JB Hi-Fi technical exercise.

## Requirements
- Visual Studio 2022
- NPM
- OpenWeatherMap API Key
- Azure AI Key (from me)

## Installation

#### Backend

Add the following `local.settings.json` file in the `Backend` folder, replacing the API keys with your own:

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated"
  },
  "OpenWeatherService": {
    "ApiKey": ">>>INSERT_API_KEY_HERE<<<",
    "WeatherForecastURL": "http://api.openweathermap.org/data/2.5/weather"
  },
  "WeatherAIService": {
    "ApiKey": ">>>INSERT_API_KEY_HERE<<<",
    "Endpoint": "https://alber-m46vtko1-eastus2.openai.azure.com/openai/deployments/gpt-4o-mini/chat/completions?api-version=2024-02-15-preview"
  },
  "Host": {
    "LocalHttpPort": 7278,
    "CORS": "*"
  }
}
```

#### Frontend
To start the front end, navigate to the ```Frontend``` folder in your terminal, or open the ```Frontend``` folder in VSCode.

Then run the following commands:

```bash
npm install
npm start
```
It should be running on ```localhost:3000.```

## Usage

To access the frontend navigate to ```http://localhost:3000/``` in the browser.

Once you have entered a valid city and country (country code according to ISO 3166 2 letter format) and click submit, it will show the description of the weather report on the screen.

## Summary of Changes

- Upgraded to React 19.
- Refactored backend to use Azure Functions.
- Created Azure AI service in Azure.
- Refactored frontend to use custom hooks for fetching the weather data.
- Show a message when the rate limit in Azure is reached.

## Known Issues

- Two package vulnerabilities in the backend project.
- Hot reload does not seem to be working after upgrading to React 19 and not upgrading react-scripts (previous attempts to upgrade react-scripts have been too time-consuming).
- Frontend does not clear "What to Wear" and "Recommendations" after the city is changed.
- Loading states for "What to Wear" and "Recommendations" are not shown.

These issues were identified but not resolved due to time constraints. Despite these, the core functionality of the application remains intact and operational.