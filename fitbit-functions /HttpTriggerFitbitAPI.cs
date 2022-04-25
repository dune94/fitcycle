using System;
using System.Globalization;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.Text.Json;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Fitbit;
using static FitbitDatabase.FitbitCosmos;

namespace functions
{
    public static class HttpTriggerFitbitQueryLoadAPI
    {
        [FunctionName("HttpTriggerFitbitQueryLoadAPI")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HttpTriggerFitbitQueryLoadAPI function processed a request.");
            
            string rootId = await GetDatabaseDataRootInit();

            Load returnLoad = new Load();

            if (!rootId.Equals(null) && !rootId.Equals("")){
                returnLoad.DataExists = "yes";
                return new OkObjectResult(returnLoad);
            }
            else {
                returnLoad.DataExists = "no";
            }

            double aggregationDurationSeconds = Double.Parse(Environment.GetEnvironmentVariable("COSMOS_AGGREGATION_DURATION_SECONDS"));
            
            string name = Environment.GetEnvironmentVariable("FITBIT_CLIENT_ID");

            using var tokenClient = new HttpClient();
            var tokenUrl = Environment.GetEnvironmentVariable("FITBIT_TOKEN_URL");

            tokenClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Environment.GetEnvironmentVariable("FITBIT_TOKEN_PROTOCOL"), Environment.GetEnvironmentVariable("FITBIT_TOKEN"));
            
            tokenClient.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

            Uri tokenUri = new Uri(Environment.GetEnvironmentVariable("FITBIT_TOKEN_URI"));

            string fitbit = req.Query["fitbit"];

            var tokenRequestContent = new FormUrlEncodedContent(new [] {
                new KeyValuePair<string, string>("clientId", name),
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("redirect_uri", tokenUri.ToString()),
                new KeyValuePair<string, string>("code", fitbit),
            });

            var tokenResponse = await tokenClient.PostAsync(tokenUrl, tokenRequestContent);
            string tokenResult = tokenResponse.Content.ReadAsStringAsync().Result;

            dynamic fitbitTokenData = JsonConvert.DeserializeObject(tokenResult);

            string fitbitToken = fitbitTokenData?.access_token;

            using var activitiesListClient = new HttpClient();
            
            var nextUrl = Environment.GetEnvironmentVariable("FITBIT_ACTIVITIES_LIST_URL");
            
            activitiesListClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", fitbitToken);
            activitiesListClient.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

            List<Root> rootClassList = new List<Root>();

            while (!nextUrl.Equals(string.Empty))
            {
                var activitiesListResponse = await activitiesListClient.GetAsync(nextUrl);
                string activitiesListResult = activitiesListResponse.Content.ReadAsStringAsync().Result;
                if (activitiesListResponse.IsSuccessStatusCode && activitiesListResult != null){
                    Root rootClass = JsonConvert.DeserializeObject<Root>(activitiesListResult);
                    if (rootClass != null)
                    {
                        rootClassList.Add(rootClass);
                        nextUrl = (rootClass.Pagination.Next != null) ? rootClass.Pagination.Next : string.Empty;
                    }
                    else 
                    {
                        nextUrl = string.Empty;
                    }
                }
                else 
                {
                    nextUrl = string.Empty;
                }
            }

            await InitDatabase();
            
            var activitiesTypesList = Environment.GetEnvironmentVariable("FITBIT_ACTIVITIES_LIST");
            var activitiesTypesMapList = Environment.GetEnvironmentVariable("FITBIT_ACTIVITIES_MAP_LIST");

            for(int r = 0; r < rootClassList.Count; r++) {

                List<Activity> activities = rootClassList[r].Activities;
                List<Activity> activitiesFiltered = new List<Activity>();

                for(int h = 0; h < activities.Count; h++) {
                    if ((Array.IndexOf(activitiesTypesList.Split(','), activities[h].ActivityName) >= 0)) {
                        activitiesFiltered.Add(activities[h]);
                    }
                }

                if (activitiesFiltered.Equals(null) || activitiesFiltered.Count.Equals(0)){
                    continue;
                }
            
                Root rootClass = new Root();
                rootClass.Id = Guid.NewGuid();
                rootClass.CreateDate = DateTime.Now.ToString("yyyyMMddHHmmss");
                rootClass.Activities = activitiesFiltered;
                rootClass.Pagination = rootClassList[r].Pagination;

                await SetDatabaseDataRoot(rootClass);
            
                for(int i = 0; i < activitiesFiltered.Count; i++) {

                    if (Array.IndexOf(activitiesTypesMapList.Split(','), activitiesFiltered[i].ActivityName) >= 0 || activitiesTypesMapList.Equals(activitiesFiltered[i].ActivityName)) {

                        using var tcxFileClient = new HttpClient();

                        var txcFileUrl = activitiesFiltered[i].TcxLink;
                        tcxFileClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", fitbitToken);
                        tcxFileClient.DefaultRequestHeaders
                            .Accept
                            .Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));

                        var tcxFileResponse = await tcxFileClient.GetAsync(txcFileUrl);
                        string tcxFileResult = tcxFileResponse.Content.ReadAsStringAsync().Result;

                        var serializer = new XmlSerializer(typeof(TcxTrainingCenterDatabase));
                        StringReader stringReader = new StringReader(tcxFileResult);
                        var tcxTrainingCenterDatabase = (TcxTrainingCenterDatabase)serializer.Deserialize(stringReader); 

                        List<TcxActivities> tcxActivities = tcxTrainingCenterDatabase.TcxActivities;
                        for(int ii = 0; ii < tcxActivities.Count; ii++) {
                            List<TcxActivity> tcxActivity = tcxActivities[ii].TcxActivity;
                            for(int iii = 0; iii < tcxActivity.Count; iii++) {
                                
                                List<TcxLap> tcxLap = tcxActivity[iii].TcxLap;
                                for(int iv = 0; iv < tcxLap.Count; iv++) {

                                    double fastestMeters = 0;
                                    double fastestSpeed = 0;
                                    double fastestKm = 0;
                                    double highestHr = 0;
                                    double averageHr = 0;
                                    double fastestKmMarker = 0;
                                    double fastestSpeedMarker = 0;
                                    double highestHrMarker = 0;
                                    
                                    Lap lap = new Lap();
                                    lap.Id = Guid.NewGuid();
                                    lap.RootId = rootClass.Id;
                                    lap.CreateDate = DateTime.Now.ToString("yyyyMMddHHmmss");
                                    lap.StartTime = tcxLap[iv].StartTime;
                                    lap.TotalTimeSeconds = tcxLap[iv].TotalTimeSeconds;
                                    lap.TotalTimeMinutes = lap.TotalTimeSeconds/60;
                                    lap.DistanceMeters = tcxLap[iv].DistanceMeters;
                                    lap.DistanceKms = tcxLap[iv].DistanceMeters/1000;
                                    lap.Calories = tcxLap[iv].Calories;
                                    lap.Intensity = tcxLap[iv].Intensity;
                                    lap.TriggerMethod = tcxLap[iv].TriggerMethod;

                                    List<TcxTrack> tcxTrack = tcxLap[iv].TcxTrack;

                                    int tcxTotal = tcxTrack.Count;
                                    
                                    for(int u = 0; u < tcxTrack.Count; u++) {
                                        
                                        List<TcxTrackpoint> tcxTrackpoint = tcxTrack[u].TcxTrackpoint;

                                        double speed = 0;
                                        double totalHr = 0;
                                        double lastSeconds = 0;
                                        double lastDistanceMeters = 0;

                                        for(int vi = 0; vi < tcxTrackpoint.Count; vi++) {

                                            DateTimeOffset dto = DateTimeOffset.Parse(tcxTrackpoint[vi].TrackPointTime, CultureInfo.InvariantCulture);
                                            DateTime dt = dto.UtcDateTime;
                                            double currentSeconds = dt.TimeOfDay.TotalSeconds;

                                            if (lastSeconds > 0){
                                                speed = ( ( tcxTrackpoint[vi].DistanceMeters - lastDistanceMeters ) * ( 3600 / (currentSeconds - lastSeconds)) ) / 1000;
                                            }
                                            else {
                                                speed = ( ( tcxTrackpoint[vi].DistanceMeters - lastDistanceMeters ) * ( 3600 / (aggregationDurationSeconds)) ) / 1000;
                                            }
                                            if (speed > fastestSpeed){
                                                fastestSpeed = speed;
                                                fastestMeters = tcxTrackpoint[vi].DistanceMeters;
                                                fastestSpeedMarker = tcxTrackpoint[vi].DistanceMeters;
                                            }
                                            lastDistanceMeters = tcxTrackpoint[vi].DistanceMeters;
                                            lastSeconds = currentSeconds;

                                            for(int vii = 0; vii < tcxTrackpoint[vi].TcxHeartRateBpm.Count; vii++) {
                                                totalHr = totalHr + tcxTrackpoint[vi].TcxHeartRateBpm[vii].HeartRateBpmValue; 
                                                if (tcxTrackpoint[vi].TcxHeartRateBpm[vii].HeartRateBpmValue > highestHr){
                                                    highestHr = tcxTrackpoint[vi].TcxHeartRateBpm[vii].HeartRateBpmValue;
                                                    highestHrMarker = tcxTrackpoint[vi].DistanceMeters;
                                                }    
                                            }
                                        }
                                        averageHr = totalHr / tcxTrackpoint.Count;
                                    }

                                    for(int v = 0; v < tcxTrack.Count; v++) {
                                        
                                        List<TcxTrackpoint> tcxTrackpoint = tcxTrack[v].TcxTrackpoint;

                                        double speed = 0;
                                        double lastSeconds = 0;
                                        double kmMeters = 1000;
                                        double lastDistanceMeters = 0;
                                        
                                        for(int vii = 0; vii < tcxTrackpoint.Count; vii++) {

                                            DateTimeOffset dto = DateTimeOffset.Parse(tcxTrackpoint[vii].TrackPointTime, CultureInfo.InvariantCulture);
                                            DateTime dt = dto.UtcDateTime;
                                            double currentSeconds = dt.TimeOfDay.TotalSeconds;

                                            if (tcxTrackpoint[vii].DistanceMeters >= kmMeters){

                                                speed = ( ( tcxTrackpoint[vii].DistanceMeters - lastDistanceMeters ) * ( 3600 / (currentSeconds - lastSeconds)) ) / 1000;
                                                lastDistanceMeters = tcxTrackpoint[vii].DistanceMeters;
                                                lastSeconds = currentSeconds;
                                                if (speed > fastestKm){
                                                    fastestKm = speed;
                                                    fastestKmMarker = tcxTrackpoint[vii].DistanceMeters;
                                                }
                                                kmMeters = kmMeters + 1000;
                                            }
                                        }
                                    }

                                    lap.FastestSpeed = fastestSpeed;
                                    lap.FastestKm = fastestKm;
                                    lap.HighestHr = highestHr;
                                    lap.AverageHr = averageHr;
                                    lap.FastestKmMarker = fastestKmMarker;
                                    lap.FastestSpeedMarker = fastestSpeedMarker;
                                    lap.HighestHrMarker = highestHrMarker;
                                    
                                    await SetDatabaseDataLap(lap);
                                    
                                    for(int v = 0; v < tcxTrack.Count; v++) {
                                        
                                        List<TcxTrackpoint> tcxTrackpoint = tcxTrack[v].TcxTrackpoint;
                                        int tcxCount = 0;
                                        double lastSeconds = 0;
                                        double lastDistanceMeters = 0;
                                        
                                        for(int vi = 0; vi < tcxTrackpoint.Count; vi++) {

                                            tcxCount = tcxCount + 1;
                                            
                                            bool createTcx = false;

                                            DateTimeOffset dto = DateTimeOffset.Parse(tcxTrackpoint[vi].TrackPointTime, CultureInfo.InvariantCulture);
                                            DateTime dt = dto.UtcDateTime;
                                            double currentSeconds = dt.TimeOfDay.TotalSeconds;

                                            if (tcxCount.Equals(1)){
                                                createTcx = true;
                                            }
                                            else {
                                                if (currentSeconds > (lastSeconds + aggregationDurationSeconds)) {
                                                    createTcx = true;
                                                }
                                                else {
                                                    createTcx = false;
                                                }
                                            }

                                            double speed = 0;
                                            if (lastSeconds > 0){
                                                speed = ( ( tcxTrackpoint[vi].DistanceMeters - lastDistanceMeters ) * ( 3600 / (currentSeconds - lastSeconds)) ) / 1000;
                                            }
                                            else {
                                                speed = ( ( tcxTrackpoint[vi].DistanceMeters - lastDistanceMeters ) * ( 3600 / (aggregationDurationSeconds)) ) / 1000;
                                            }

                                            bool fastestTcx = false;

                                            if (fastestMeters.Equals(tcxTrackpoint[vi].DistanceMeters)){
                                                createTcx = true;
                                                fastestTcx = true;
                                            }

                                            if (createTcx.Equals(true) || fastestTcx.Equals(true)) {

                                                tcxTrackpoint[vi].Id = Guid.NewGuid();
                                                tcxTrackpoint[vi].RootId = rootClass.Id;
                                                tcxTrackpoint[vi].LapId = lap.Id;
                                                tcxTrackpoint[vi].CreateDate = DateTime.Now.ToString("yyyyMMddHHmmss");
                                                tcxTrackpoint[vi].Speed = speed;

                                                if (speed.Equals(fastestSpeed) || fastestTcx.Equals(true)){
                                                    tcxTrackpoint[vi].Fastest = true;
                                                }
                                                else {
                                                    tcxTrackpoint[vi].Fastest = false;
                                                }

                                                if (tcxTrackpoint[vi].DistanceMeters >= fastestKmMarker && tcxTrackpoint[vi].DistanceMeters <= (fastestKmMarker + 1000)){
                                                    tcxTrackpoint[vi].FastestKm = true;
                                                }
                                                else {
                                                    tcxTrackpoint[vi].FastestKm = false;
                                                }

                                                lastDistanceMeters = tcxTrackpoint[vi].DistanceMeters;
                                                lastSeconds = currentSeconds;

                                                await SetDatabaseDataTcxTrackpoint(tcxTrackpoint[vi]);
                                                
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return new OkObjectResult(returnLoad);
        
        }

    }

    public static class HttpTriggerQueryFitbitPolylinesAPI
    {
        [FunctionName("HttpTriggerQueryFitbitPolylinesAPI")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HttpTriggerQueryFitbitPolylinesAPI function processed a request.");

            string lapId = req.Query["lapid"];

            List<Polylines> polylinesList = await GetDatabaseDataRootPolylines(lapId);

            string json = JsonConvert.SerializeObject(polylinesList, Formatting.Indented);

            return new OkObjectResult(json);

        }
    }

    public static class HttpTriggerQueryFitbitLapsAPI
    {
        [FunctionName("HttpTriggerQueryFitbitLapsAPI")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HttpTriggerQueryFitbitLapsAPI function processed a request.");

            List<Lap> lapsList = await GetDatabaseDataRootLaps();

            string json = JsonConvert.SerializeObject(lapsList, Formatting.Indented);

            return new OkObjectResult(json);

        }
    }

}
