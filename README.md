# Technical Exercise

JB Hi-Fi technical exercise.

## Requirements
- Visual Studio 2019
- NPM
- OpenWeatherMap API Key

## Installation

#### Backend

You will need to add a valid API key for OpenWeatherMap in line 10 of ```Backend/WeatherData/appsettings.json```

(Using this rudimentary way of storing secrets just to make it easy to run)

Example: 
```"OPEN_WEATHER_API_KEY": ">>>INSERT_API_KEY_HERE<<<",```

You will then need to start the backend server using IIS in Visual Studio 2019, using the ```WeatherData.sln``` solution. It should be run on localhost port 44396.

Once the backend is running, if you like you can verify it is running correctly making a request (using POSTMAN or similar) to ```https://localhost:44396/weatherforecast/forecast?city=melbourne&countryCode=au```, and adding a valid API key as a ClientId header.

(ClientId:'Bob')

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

If you make more than 5 requests in one hour you will get a message on the screen that the rate limit has been exceeded.

The ClientId (API Key) is hardcoded in App.js line 14. It is set to one of the 5 valid API keys, you can check that a 401 is received when the ClientId is changed, and a relevant error message appears on the screen.

There will be error messages received for when the rate limit is exceeded or the ClientId is not Authorised. (Or no API key is provided)

Not providing city will result in a 400 error.

## Testing

To run tests for the Backend, right click on the WeatherData.Tests project in Visual Studio and click Run Tests.

To run tests on the Frontend, run ```npm test``` from the Frontend folder.