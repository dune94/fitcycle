FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS installer-env

COPY . /src/dotnet-function-app
RUN cd /src/dotnet-function-app && \
    mkdir -p /home/site/wwwroot && \
    dotnet publish *.csproj --output /home/site/wwwroot

# To enable ssh & remote debugging on app service change the base image to the one below
# FROM mcr.microsoft.com/azure-functions/dotnet:3.0-appservice
FROM mcr.microsoft.com/azure-functions/dotnet:3.0
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true \
    COSMOS_ENDPOINT_URI=https://host.docker.internal:8081 \
    COSMOS_PRIMARY_KEY=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw== \
    COSMOS_AGGREGATION_DURATION_SECONDS=20.000000 \
    COSMOS_RU_LOW=2000 \
    COSMOS_RU_HIGH=20000 \
    COSMOS_COMMIT_SLEEP=300 \
    COSMOS_TIMEOUT=60000 \
    COSMOS_PAUSE_BETWEEN_FAILURES=1 \
    COSMOS_FAILURE_RETRIES=3 \
    FITBIT_TOKEN_URL=https://api.fitbit.com/oauth2/token \
    FITBIT_TOKEN_PROTOCOL=Basic \
    FITBIT_TOKEN=MjNCR1I5OjNiOGQ0Mjk0ZDk2YzhmYTczMmM5NWY4NjYzNTQxOGI3 \
    FITBIT_CLIENT_ID=23BGR9 \
    FITBIT_ACTIVITIES_TYPE=Cycling \
    FITBIT_ACTIVITIES_LIST=Bike \
    FITBIT_ACTIVITIES_MAP_LIST=Bike \
    FITBIT_TOKEN_URI=http://127.0.0.1:4200 \
    FITBIT_REDIRECT_URI=http%3A%2F%2F127.0.0.1%3A4200%2F \
    FITBIT_ACTIVITIES_LIST_URL=https://api.fitbit.com/1/user/-/activities/list.json?afterDate=2021-10-13&sort=desc&offset=0&limit=100 \
    FITBIT_PROFILE_URL=https://api.fitbit.com/1/user/-/profile.json \
    FITBIT_URL=https://www.fitbit.com/oauth2/authorize?response_type=code&client_id=FITBIT_CLIENT_ID&redirect_uri=FITBIT_REDIRECT_URI&scope=activity%20heartrate%20location%20nutrition%20profile%20settings%20sleep%20social%20weight&expires_in=604800


COPY --from=installer-env ["/home/site/wwwroot", "/home/site/wwwroot"]