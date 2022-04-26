# fitcycle
# Fitbit cycling dashboard app (MVP)

## Link to Video
[Application Overview YouTube](https://www.youtube.com/watch?v=ukVpg2f1Yxg)

## Link to Architecture Diagram
[Architecture](https://viewer.diagrams.net/?tags=%7B%7D&highlight=0000ff&edit=_blank&layers=1&nav=1&title=fitcycle-mvp-architecture.drawio#R5VjZbuIwFP0aNDMPHZGErY8QoEUqLRJd52VkHJO4NXHGcVj69XOdOJCQlEEjKEh9aX2Pl8Tn%2BJ5rUrHs2fJKoMAbcoewill1lhWrWzFNw2qZ8E8hqwRpWo0EcAV19KANMKbvRINVjUbUIWFuoOScSRrkQcx9n2CZw5AQfJEfNuUs%2F9QAuaQAjDFiRfSJOtJL0Fa9usGvCXW99MlGVffMUDpYA6GHHL7IQFavYtmCc5m0ZkubMEVeyksyr%2F9B7%2FrFBPHlPhNqL%2Fbzrxdzaj7hVm8wehh0L4MLLcYcsUhvWL%2BsXKUMEAcI0SEX0uMu9xHrbdCO4JHvEPWYKkSbMTecBwAaAL4SKVdaXRRJDpAnZ0z3hhIJ2VZqAYAZCkOKU7hPWTqsuGNNQsgjgcmObaYnBwmXyB3j9O7VljMP0HxeET4jUqxggCAMSTrPnxGkj5q7HrdRAxpakHJxrg3j9%2BvyzpyT0Z%2BLHnth7db44iRakCWVz5n2i1rqZ11H3aVeOQ5WaeDDdp%2BzQWaWCjfT4iidd1BBSyk0TiVod4TvFzQMbxe12yf%2FPrzGj9GF9TUE3WL7YAqXcvpZgu56yYx9tn03YkgA2O9VzAaDV%2B9MIGy4qjVEkgiK1LoPg8JhWHhUknGAYl4WUE%2FzQqIwSCrclC7Vgfg4feZESLLceeDT3rTO6vJ8qcNFptZpyMuUuRQ7OKHNr1GPzD3rkXVW9lU%2FH%2Fsy%2F8%2B%2FzE80sH01rp3SwMyigb1Hgij7inwsKfdDaLdHZ2lWZu3EbmUV2LN5OOOKsm6nwBj8AAhUE68YhZwQFrDyDxYnSfbcTNYAwm9unFN3kYRlSOpYSfoY9UMxXc8xbZXUheaRmC61nsYpraeas57mntZj5K2n%2BYnWU9vTesxTWk%2BtkDx9KjtUJn7zLSzenr63wZHmVFKiMgz5DvwdQspAtiGJfpyDRa0vUCewqF0%2FgDIsX3HuMuXwNuORpnBt8gXKR5wpuyLhWdJrWKemt1p2ivEKxwwPH0cV9VVmm9W2wIo7LONSu0UrUCG3mGTU9VXhAOKIAEARRjFibd0xo46TWB6BQoAm8VLKUwJOfRnvud6p1LtqLXC5tFjEV1rB34jNGYd1uz5XBaUzhSvuFnQA7Zpb0hWVuyxRzjqacsWrj3IRNU1lA4ggEJZEJchU8Jm6EaX2pBPHVhbrEV8RE4VePBTo5mpE9hqwrf6XUdxqbZlhs6i51SgR3TzWNaLohmtR7wZd%2B0AWV2CohMe9Le54BQTCzdfnuC%2FzDd%2Fq%2FQU%3D)

## Local Setup Instructions

_Please note this MVP application is running locally on Windows 10, WSL2 Ubuntu._

### Installs

Docker Desktop was installed locally.

### Google Cloud Account

A Google Cloud account is required with "Maps JavaScript API" enabled.
The API Key for this API will be required to set in the Frontend environment variables (see below).

### Fitbit Developer Account

Setup a new Fitbit developer account (dev.fitbit.com => register an app).

Upon creating a new developer account and manage an application entry, a new OAuth 2.0 Client ID will be generated. 
This Client ID is required in the environment variables configuration, please see below.

The standard redirect URL for local development is:

http://127.0.0.1:4200/


### Docker Containers

#### Angular (Frontend)

From the ~/angular-maps/angular-google-maps-polyline directory:

First, build the image ==> 
`docker build -t  angular-google-maps-polyline .`

Next, run the container ==> 
`docker run -p 4200:4200 angular-google-maps-polyline`


#### Fitbit-Functions (Backend)

From the `~/fitbit-functions` directory.

First, build the image ==> 
`docker build -t httptriggerfitbitapi .`

Next, run the container ==>
`docker run -p 8080:80 httptriggerfitbitapi`


#### Cosmos DB (Database)

Run the Cosmos Emulator. 

`docker run -p 8081:8081 -p 10251:10251 -p 10252:10252 -p 10253:10253 -p 10254:10254  -m 3g --cpus=2.0 --name=test-linux-emulator -e   AZURE_COSMOS_EMULATOR_PARTITION_COUNT=10 -e AZURE_COSMOS_EMULATOR_ENABLE_DATA_PERSISTENCE=true -e AZURE_COSMOS_EMULATOR_IP_ADDRESS_OVERRIDE=$ipaddr -it mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator`

NOTE: 
The .wslconfig file settings used to accomodate the Cosmos Emulator is:

`
[wsl2]
memory=8GB
processors=2
swap=0
localhostForwarding=true`


### Useful Links

Application link, assuming standard Angular local configuration ==> `localhost:4200`

The Fitbit Client ID from Fitbit Developer account must be filled into the application link below (replace {CLIENTID} with the value):

`https://www.fitbit.com/oauth2/authorize?response_type=code&client_id={CLIENTID}&redirect_uri=http%3A%2F%2F127.0.0.1%3A4200%2F&scope=activity%20heartrate%20location%20nutrition%20profile%20settings%20sleep%20social%20weight&expires_in=604800`


Access Cosmos Emulator

`https://localhost:8081/_explorer/index.html`


#### Fitcycle API's

`http://localhost:8080/api/httptriggerqueryfitbitlapsapi`

Example response:

`
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
  },`

`http://localhost:8080/api/httptriggerqueryfitbitpolylinesapi?lapid=df6ee36d-fa45-44d1-8948-15db381d6bf7`

Example response:

`
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
          },`

## Environment Variables - Backend (fitbit-functions)

### Azure Functions Variables

`AzureWebJobsScriptRoot=/home/site/wwwroot`

`AzureFunctionsJobHost__Logging__Console__IsEnabled=true`

### Cosmos Emulator Variables

`COSMOS_ENDPOINT_URI=https://host.docker.internal:8081` 

`COSMOS_PRIMARY_KEY=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==`

`COSMOS_AGGREGATION_DURATION_SECONDS=20.000000`
Notes: This value drives the polylines aggregation only after every 20 seconds are speed changes detected and a possible new polyline created.

`COSMOS_RU_LOW=2000` 

`COSMOS_RU_HIGH=20000` 
Notes: Cosmos DB collections (Root, Lap, Trackpoint) are divided into low and high consumers, and the RU is set accordingly for dev purposes. 

`COSMOS_COMMIT_SLEEP=300` 

`COSMOS_TIMEOUT=60000` 

`COSMOS_PAUSE_BETWEEN_FAILURES=1` 

`COSMOS_FAILURE_RETRIES=3` 

### Fitbit Variables

`FITBIT_TOKEN_URL=https://api.fitbit.com/oauth2/token` 

`FITBIT_TOKEN_PROTOCOL=Basic` 

`FITBIT_TOKEN=MjNCR1I5OjNiOGQ0Mjk0ZDk2YzhmYTczMmM5NWY4NjYzNTQxOGI3`

`FITBIT_CLIENT_ID=23BGR9`
Notes: This client ID variable should be assigned after creating a new Fitbit developer account.

`FITBIT_ACTIVITIES_TYPE=Cycling`

`FITBIT_ACTIVITIES_LIST=Bike`

`FITBIT_ACTIVITIES_MAP_LIST=Bike`

`FITBIT_TOKEN_URI=http://127.0.0.1:4200`

`FITBIT_REDIRECT_URI=http%3A%2F%2F127.0.0.1%3A4200%2F`

`FITBIT_ACTIVITIES_LIST_URL=https://api.fitbit.com/1/user/-/activities/list.json?afterDate=2021-10-13&sort=desc&offset=0&limit=100`
Notes: Currently, this URL variable includes a hard coded afterDate param. A future change to this app will parameterize this date value.

`FITBIT_URL=https://www.fitbit.com/oauth2/authorize?response_type=code&client_id=FITBIT_CLIENT_ID&redirect_uri=FITBIT_REDIRECT_URI&scope=activity%20heartrate%20location%20nutrition%20profile%20settings%20sleep%20social%20weight&expires_in=604800`

## Environment Variables - Frontend

In ~/angular-maps/angular-google-maps-polyline/src/environments:

Replace apiKey value with Google Cloud account API Key value.

