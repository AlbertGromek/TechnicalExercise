# Technical Exercise

JB Hi-Fi technical exercise.

## Requirements
- Visual Studio 2022
- NPM
- OpenWeatherMap API Key
- Azure AI Key (from me)

## My aim

To make the project a bit closer to the code I would write today compared to the very basic code from 3 years ago. To do this I rewrote the backend into an Azure Function trying to follow clean architecture, and also refactored the frontend to be more modular and easy to build on. I also wanted to do something fun/interesting, so for that part I added a call to Azure's Open AI Service to get some recommendations on what to do or wear.

## Changes in 2024
- Added a call to an Azure Open AI instance to get recommendations on what to wear or what do do in the given location with the current weather report
- Instead of the rate limit part of the exercise I have instead passed the retry at headers returned from Azure to let the user know when they can make the next request, since the free tier is limited to one request per minute
- Rewritten the backend as an Azure function in .NET8 
- I did not re-write the rate limit part of the exercise (sorry, can look at my old code if you want an implementation :D) because I thought it wouldnt be as interesting as the Azure AI stuff
- Refactored a lot of the front end code to use smaller components and a custom hook for calling the weather API, and storing the result in context
- Upgraded frontend to React 19 (didn't touch react scripts/webpack stuff because its a pain, would redo the frontend from scratch but ran out of time. Hot reload doesnt seem to be working after the upgrade but I've left it as is)
- Exported a generated typescript client from the API to be used in the frontend (using nswag), which gives me easy and type safe API requests using the generated client, instead of having to write my own client manually and the types. Also makes it a lot easier to add more stuff. The caveat is I didn't realise it doesn't integrate so easily with Azure functions until I was almost done, so the annoying part is you have to manually update the swagger.json (which nswag will then use to generate) after making code changes that you want reflected in the typescript client.

## Installation

#### Backend

Add the following `local.settings.json` file in the `Weather.FunApp` project, replacing the weather API key with a real one, and I will send you a free tier Azure API key:

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

Then you can run it normally.

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

Once you have entered a valid city and country (country code according to ISO 3166 2 letter format) and click submit, it will show the description of the weather report on the screen. You can then click the other two buttons to get a recommendation on what to do or wear. (limited to one request per minute)

## Summary of Changes

- Upgraded to React 19.
- Refactored backend to use Azure Functions.
- Created Azure Open AI service in Azure.
- Refactored frontend to use custom hooks for fetching the weather data.
- Show a message when the rate limit in Azure is reached.

## Known Issues I didn't have time to fix

- The nswag generator from the Azure functions project build *may* fail if the build process isn't using the same version of .NET as specified in the nswag file. My code is .NET 8 but apparently my computer is building it using .NET 9 for some reason. If the build fails that's probably why, you can comment out the following section from the WeatherFunctions .csproj if that happens and then typescript client won't get regenerated.

```
	<Target Name="GenerateTypeScriptClient" BeforeTargets="Build">
		<Exec Command="nswag run nswag.json" />
	</Target>
```
Or alternatively can try to change the .NET version in the nswag.json file in the functions project 
```
  "runtime": "Net90",
```
- Two package vulnerabilities in the backend project. (see https://github.com/Azure/azure-functions-openapi-extension/issues/671)
- Hot reload does not seem to be working after upgrading to React 19 and not upgrading react-scripts (hisotircal attempts to upgrade react-scripts in other projects have been time-consuming).
- Frontend does not clear "What to Wear" and "Recommendations" after the city is changed.
- Loading states for "What to Wear" and "Recommendations" are not shown, and Azure is a bit slow with the AI stuff.
