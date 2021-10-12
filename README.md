# Technical Exercise

Technical Exercise built according to the specifications given :) 

## Installation

Installation is pretty basic, just need to run some npm commands to get the frontend running and start the IIS server in Visual Studio.

## Backend Setup 
Open the ```WeatherData.sln``` solution in Visual Studio 2019.

Start the project using IIS.

Check that the project is running on localhost port 44396.

## Frontend Setup
To start the front end, navigate to the frontend folder in your terminal, or open the Frontend folder in VSCode.

Then run the following commands:

```bash
npm install
npm start
```

## Usage

NOTE: I set my API rate limit to 5 requests per minute, not the 5 per hour required in the spec. I figured it is easier for testing for myself, so it might be easier for you to test also. Please let me know if you'd like me to change it to 5/hour and I can push up the changes!

To access the frontend navigate to ```http://localhost:3000/``` in the browser.

Once you have entered a valid city and country (country code according to ISO 3166 2 letter format) and click submit, it will show the description of the weather report on the screen. 

If you make more than 5 requests in one minute you will get a message on the screen that the rate limit has been exceeded.

The ClientId (API Key) is hardcoded in App.js line 14. It is set to one of the 5 valid API keys, you can check that a 401 is received when the ClientId is changed, and a relevant error message appears on the screen.

If you'd like to test the Backend service using something like Postman you can make a request using the format below:

```https://localhost:44396/weatherforecast/forecast?city=melbourne&countryCode=au```

Passing the API key in the ```'ClientId'``` header.
There will be error messages received for when the rate limit is exceeded or the ClientId is not Authorised. (Or no API key is provided)
