# fitcycle
Fitbit cycling dashboard app (MVP)

Local Setup Instructions

Please note this MVP application is running locally on Windows 10, WSL2 Ubuntu.


FitBit Developer Account:

Setup a new Fitbit developer account (dev.fitbit.com => register an app).

A new OAuth 2.0 Client ID will be generated. 
This is necessary for setup in the URL noted below.
Redirect URL for local development is:
http://127.0.0.1:4200/


Containers:

Angular (Frontend)

From ~/angular-maps/angular-google-maps-polyline directory:

docker build -t  angular-google-maps-polyline .

docker run -p 4200:4200 angular-google-maps-polyline


Fitbit-Functions (Backend)

From ~/fitbit-functions directory:

docker build -t httptriggerfitbitapi .

docker run -p 8080:80 httptriggerfitbitapi


Cosmos DB (Database)

docker run -p 8081:8081 -p 10251:10251 -p 10252:10252 -p 10253:10253 -p 10254:10254  -m 3g --cpus=2.0 --name=test-linux-emulator -e   AZURE_COSMOS_EMULATOR_PARTITION_COUNT=10 -e AZURE_COSMOS_EMULATOR_ENABLE_DATA_PERSISTENCE=true -e AZURE_COSMOS_EMULATOR_IP_ADDRESS_OVERRIDE=$ipaddr -it mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator


Userful links:

Application link (assuming standard Angular local configuration localhost:4200):

Client ID from Fitbit Developer account must be filled into the application link.

https://www.fitbit.com/oauth2/authorize?response_type=code&client_id={CLIENTID}&redirect_uri=http%3A%2F%2F127.0.0.1%3A4200%2F&scope=activity%20heartrate%20location%20nutrition%20profile%20settings%20sleep%20social%20weight&expires_in=604800


Access Cosmos Emulator:

https://localhost:8081/_explorer/index.html


API's:

Get all Laps (Cycling workouts)
http://localhost:8080/api/httptriggerqueryfitbitlapsapi

Example response:

[
  {
    "id": "df6ee36d-fa45-44d1-8948-15db381d6bf7",
    "RootId": "4765c586-9c31-4179-b6d9-18889d7e49a2",
    "CreateDate": "20220413180311",
    "StartTime": "11/10/2021 18:12:18",
    "TotalTimeSeconds": 3568.0,
    "TotalTimeMinutes": 59.5,
    "DistanceMeters": 18360.22,
    "DistanceKms": 18.4,
    "Calories": 310,
    "Intensity": null,
    "TriggerMethod": null,
    "DropDown": "11/10/2021 18",
    "Activity": null,
    "FastestSpeed": 30.0,
    "FastestSpeedMarker": 16.0,
    "FastestKm": 26.8,
    "FastestKmMarker": 9.0,
    "HighestHr": 112.0,
    "HighestHrMarker": 3.0,
    "AverageHr": 91.6
  },

http://localhost:8080/api/httptriggerqueryfitbitpolylinesapi?lapid=df6ee36d-fa45-44d1-8948-15db381d6bf7

Example response:

[
  {
    "Id": "fe0571aa-090a-4785-9ae8-9acc8129b4ab",
    "path": [
      {
        "Id": "02c8344f-b353-4b3d-9795-67c9a58cac1c",
        "point": [
          {
            "latitude": 43.23983824253082,
            "longitude": -79.72222661972046,
            "fastest": false,
            "Speed": null,
            "icon": null
          },
          {
            "latitude": 43.23983824253082,
            "longitude": -79.72222661972046,
            "fastest": false,
            "Speed": null,
            "icon": null
          },





