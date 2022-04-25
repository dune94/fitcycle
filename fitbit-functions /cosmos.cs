using System;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Linq;
using System.Text;
using System.Net.Http;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Documents;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Fitbit;
using Utils;
using static Utils.RetryHelper;


namespace FitbitDatabase
{
    class FitbitCosmos: IDisposable
    {

        // The Azure Cosmos DB endpoint for running this sample.
        private static readonly string EndpointUri = Environment.GetEnvironmentVariable("COSMOS_ENDPOINT_URI");
        // The primary key for the Azure Cosmos account.
        private static readonly string PrimaryKey = Environment.GetEnvironmentVariable("COSMOS_PRIMARY_KEY");
        
        // The Cosmos client instance
        private CosmosClient cosmosClient;

        // The database we will create
        private Database database;

        // The container we will create.
        private Container containerRoot;
        private Container containerLap;
        private Container containerTcxTrackpoint;
        
        bool disposed = false;

        // The name of the database and container we will create
        private string databaseId = "Fitbit";
        private static string containerRootId = "Root";
        private static string containerLapId = "Lap";
        private static string containerTcxTrackpointId = "Trackpoint";

        private static int cosmosRuLow = Int32.Parse(Environment.GetEnvironmentVariable("COSMOS_RU_LOW"));
        private static int cosmosRuHigh = Int32.Parse(Environment.GetEnvironmentVariable("COSMOS_RU_HIGH"));


        private static TimeSpan timeOut = TimeSpan.FromSeconds(Int32.Parse(Environment.GetEnvironmentVariable("COSMOS_TIMEOUT")));

        CosmosClientOptions cosmosClientOptions = new CosmosClientOptions()
        {
            HttpClientFactory = () =>
            {
                HttpMessageHandler httpMessageHandler = new HttpClientHandler()
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };
                HttpClient httpClient = new HttpClient(httpMessageHandler);
                httpClient.Timeout = timeOut;
                return httpClient;
            },
            ConnectionMode = ConnectionMode.Gateway,
            RequestTimeout = timeOut
        };

        IndexingPolicy indexingRootPolicy = new IndexingPolicy
        {
            IndexingMode = IndexingMode.Consistent,
            Automatic = true,
            IncludedPaths =
            {
                new IncludedPath
                {
                    Path = "/*"
                }
            }
        };

        IndexingPolicy indexingTcxTrackpointPolicy = new IndexingPolicy
        {
            IndexingMode = IndexingMode.Consistent,
            Automatic = true,
            IncludedPaths =
            {
                new IncludedPath
                {
                    Path = "/*"
                }
            }
        };

        IndexingPolicy indexingLapPolicy = new IndexingPolicy
        {
            IndexingMode = IndexingMode.Consistent,
            Automatic = true,
            IncludedPaths =
            {
                new IncludedPath
                {
                    Path = "/*"
                }
            }
        };

        public static async Task InitDatabase()
        {
            try
            {
                FitbitCosmos fitbitCosmos = new FitbitCosmos();
                await fitbitCosmos.DatabaseAsync();
            }
            catch (CosmosException de)
            {
                Exception baseException = de.GetBaseException();
                Console.WriteLine("{0} error occurred: {1}\n", de.StatusCode, de);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}\n", e);
            }
            finally
            {
            }
        }

        public static async Task SetDatabaseDataRoot(Root root)
        {
            try
            {
                FitbitCosmos fitbitCosmos = new FitbitCosmos();
                await fitbitCosmos.DataRootAsync(root);
            }
            catch (CosmosException de)
            {
                Exception baseException = de.GetBaseException();
                Console.WriteLine("{0} error occurred: {1}\n", de.StatusCode, de);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}\n", e);
            }
            finally
            {
            }
        }

        public static async Task SetDatabaseDataTcxTrackpoint(TcxTrackpoint tcxTrackpoint)
        {
            try
            {
                FitbitCosmos fitbitCosmos = new FitbitCosmos();
                await fitbitCosmos.DataTcxTrackpointAsync(tcxTrackpoint);
            }
            catch (CosmosException de)
            {
                Exception baseException = de.GetBaseException();
                Console.WriteLine("{0} error occurred: {1}\n", de.StatusCode, de);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}\n", e);
            }
            finally
            {
            }
        }

        public static async Task SetDatabaseDataLap(Lap lap)
        {
            try
            {
                FitbitCosmos fitbitCosmos = new FitbitCosmos();
                await fitbitCosmos.DataLapAsync(lap);
            }
            catch (CosmosException de)
            {
                Exception baseException = de.GetBaseException();
                Console.WriteLine("{0} error occurred: {1}\n", de.StatusCode, de);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}\n", e);
            }
            finally
            {
            }
        }

        public static async Task<List<TcxTrackpoint>> GetDatabaseDataRoot(string _query)
        {
            List<TcxTrackpoint> tcxTrackpointList = new List<TcxTrackpoint>();
            try
            {
                FitbitCosmos fitbitCosmos = new FitbitCosmos();
                tcxTrackpointList = await fitbitCosmos.GetDataRoot(_query);
            }
            catch (CosmosException de)
            {
                Exception baseException = de.GetBaseException();
                Console.WriteLine("{0} error occurred: {1}\n", de.StatusCode, de);
                return tcxTrackpointList;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}\n", e);
                return tcxTrackpointList;
            }
            finally
            {
            }
            return tcxTrackpointList;
        }

        public static async Task<List<Polylines>> GetDatabaseDataRootPolylines(string lapId)
        {
            List<Polylines> polylinesList = new List<Polylines>();
            try
            {
                FitbitCosmos fitbitCosmos = new FitbitCosmos();
                polylinesList = await fitbitCosmos.GetDataRootPolylines(lapId);
            }
            catch (CosmosException de)
            {
                Exception baseException = de.GetBaseException();
                Console.WriteLine("{0} error occurred: {1}\n", de.StatusCode, de);
                return polylinesList;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}\n", e);
                return polylinesList;
            }
            finally
            {
            }
            return polylinesList;
        }

        public static async Task<List<Lap>> GetDatabaseDataRootLaps()
        {
            List<Lap> lapsList = new List<Lap>();
            try
            {
                FitbitCosmos fitbitCosmos = new FitbitCosmos();
                lapsList = await fitbitCosmos.GetDataRootLaps();
            }
            catch (CosmosException de)
            {
                Exception baseException = de.GetBaseException();
                Console.WriteLine("{0} error occurred: {1}\n", de.StatusCode, de);
                return lapsList;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}\n", e);
                return lapsList;
            }
            finally
            {
            }
            return lapsList;
        }

        public static async Task<string> GetDatabaseDataRootInit()
        {
            string rootId = null;
            try
            {
                FitbitCosmos fitbitCosmos = new FitbitCosmos();
                rootId = await fitbitCosmos.GetDataRootInit();
            }
            catch (CosmosException de)
            {
                Exception baseException = de.GetBaseException();
                Console.WriteLine("{0} error occurred: {1}\n", de.StatusCode, de);
                return rootId;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}\n", e);
                return rootId;
            }
            finally
            {
            }
            return rootId;
        }

        /// <summary>
        /// Entry point to call methods that operate on Azure Cosmos DB resources in this sample
        /// </summary>
        public async Task DatabaseAsync()
        {
            // Create a new instance of the Cosmos Client
            this.cosmosClient = new CosmosClient(EndpointUri, PrimaryKey, cosmosClientOptions);
            await this.CreateDatabaseAsync();
            await this.CreateContainerRootAsync();
            await this.CreateContainerLapAsync();
            await this.CreateContainerTcxTrackpointAsync();
        }

        /// <summary>
        /// Create the database if it does not exist
        /// </summary>
        private async Task CreateDatabaseAsync()
        {
            // Create a new database
            var maxRetryAttempts = Int32.Parse(Environment.GetEnvironmentVariable("COSMOS_FAILURE_RETRIES"));  
            var pauseBetweenFailures = TimeSpan.FromSeconds(Int32.Parse(Environment.GetEnvironmentVariable("COSMOS_PAUSE_BETWEEN_FAILURES")));  
            
            await RetryHelper.RetryOnExceptionAsync<CosmosException>
                (maxRetryAttempts, pauseBetweenFailures, async () => {
                this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
            }); 
        }

        private async Task CreateContainerRootAsync()
        {
            ContainerProperties containerRootProperties = new ContainerProperties(containerRootId, "/CreateDate")
            {
                IndexingPolicy = indexingRootPolicy
            };
            var maxRetryAttempts = Int32.Parse(Environment.GetEnvironmentVariable("COSMOS_FAILURE_RETRIES"));  
            var pauseBetweenFailures = TimeSpan.FromSeconds(Int32.Parse(Environment.GetEnvironmentVariable("COSMOS_PAUSE_BETWEEN_FAILURES")));  
            
            await RetryHelper.RetryOnExceptionAsync<CosmosException>
                (maxRetryAttempts, pauseBetweenFailures, async () => {
                this.containerRoot = await this.database.CreateContainerIfNotExistsAsync(containerRootProperties, cosmosRuLow);
            }); 
        }

        private async Task CreateContainerTcxTrackpointAsync()
        {
            // Create a new container
            ContainerProperties containerTcxTrackpointProperties = new ContainerProperties(containerTcxTrackpointId, "/RootId")
            {
                IndexingPolicy = indexingTcxTrackpointPolicy
            };
            
            var maxRetryAttempts = Int32.Parse(Environment.GetEnvironmentVariable("COSMOS_FAILURE_RETRIES"));  
            var pauseBetweenFailures = TimeSpan.FromSeconds(Int32.Parse(Environment.GetEnvironmentVariable("COSMOS_PAUSE_BETWEEN_FAILURES")));  
            
            await RetryHelper.RetryOnExceptionAsync<CosmosException>
                (maxRetryAttempts, pauseBetweenFailures, async () => {
                this.containerTcxTrackpoint = await this.database.CreateContainerIfNotExistsAsync(containerTcxTrackpointProperties, cosmosRuHigh);
            }); 
        }

        private async Task CreateContainerLapAsync()
        {
            // Create a new container
            ContainerProperties containerLapProperties = new ContainerProperties(containerLapId, "/RootId")
            {
                IndexingPolicy = indexingLapPolicy
            };
            
            var maxRetryAttempts = Int32.Parse(Environment.GetEnvironmentVariable("COSMOS_FAILURE_RETRIES"));
            var pauseBetweenFailures = TimeSpan.FromSeconds(Int32.Parse(Environment.GetEnvironmentVariable("COSMOS_PAUSE_BETWEEN_FAILURES")));  
            
            await RetryHelper.RetryOnExceptionAsync<CosmosException>
                (maxRetryAttempts, pauseBetweenFailures, async () => {
                this.containerLap = await this.database.CreateContainerIfNotExistsAsync(containerLapProperties, cosmosRuLow);
            }); 
        }

        private async Task DataRootAsync(Root root)
        {
            this.cosmosClient = new CosmosClient(EndpointUri, PrimaryKey, cosmosClientOptions);
            
            var maxRetryAttempts = Int32.Parse(Environment.GetEnvironmentVariable("COSMOS_FAILURE_RETRIES"));  
            var pauseBetweenFailures = TimeSpan.FromSeconds(Int32.Parse(Environment.GetEnvironmentVariable("COSMOS_PAUSE_BETWEEN_FAILURES")));  
            
            await RetryHelper.RetryOnExceptionAsync<CosmosException>
                (maxRetryAttempts, pauseBetweenFailures, async () => {
                this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
            }); 
            
            await this.CreateContainerRootAsync();

            await RetryHelper.RetryOnExceptionAsync<CosmosException>
                (maxRetryAttempts, pauseBetweenFailures, async () => {
                ItemResponse<Root> rootResponse = await this.containerRoot.CreateItemAsync<Root>(root);
            }); 
            Thread.Sleep(int.Parse(Environment.GetEnvironmentVariable("COSMOS_COMMIT_SLEEP")));
        }

        private async Task DataTcxTrackpointAsync(TcxTrackpoint tcxTrackpoint)
        {
            this.cosmosClient = new CosmosClient(EndpointUri, PrimaryKey, cosmosClientOptions);

            var maxRetryAttempts = Int32.Parse(Environment.GetEnvironmentVariable("COSMOS_FAILURE_RETRIES"));  
            var pauseBetweenFailures = TimeSpan.FromSeconds(Int32.Parse(Environment.GetEnvironmentVariable("COSMOS_PAUSE_BETWEEN_FAILURES")));  
            
            await RetryHelper.RetryOnExceptionAsync<CosmosException>
                (maxRetryAttempts, pauseBetweenFailures, async () => {
                this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
            }); 

            await this.CreateContainerTcxTrackpointAsync();

            await RetryHelper.RetryOnExceptionAsync<CosmosException>
                (maxRetryAttempts, pauseBetweenFailures, async () => {
                ItemResponse<TcxTrackpoint> tcxTrackpointResponse = await this.containerTcxTrackpoint.CreateItemAsync<TcxTrackpoint>(tcxTrackpoint);
                Thread.Sleep(int.Parse(Environment.GetEnvironmentVariable("COSMOS_COMMIT_SLEEP")));
            }); 
        }

        private async Task DataLapAsync(Lap lap)
        {
            this.cosmosClient = new CosmosClient(EndpointUri, PrimaryKey, cosmosClientOptions);

            var maxRetryAttempts = Int32.Parse(Environment.GetEnvironmentVariable("COSMOS_FAILURE_RETRIES"));  
            var pauseBetweenFailures = TimeSpan.FromSeconds(Int32.Parse(Environment.GetEnvironmentVariable("COSMOS_PAUSE_BETWEEN_FAILURES")));  
            
            await RetryHelper.RetryOnExceptionAsync<CosmosException>
                (maxRetryAttempts, pauseBetweenFailures, async () => {
                this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
            }); 

            await this.CreateContainerLapAsync();

            await RetryHelper.RetryOnExceptionAsync<CosmosException>
                (maxRetryAttempts, pauseBetweenFailures, async () => {
                ItemResponse<Lap> lapResponse = await this.containerLap.CreateItemAsync<Lap>(lap);
                Thread.Sleep(int.Parse(Environment.GetEnvironmentVariable("COSMOS_COMMIT_SLEEP")));
            }); 
        }

        private async Task<List<TcxTrackpoint>> GetDataRoot (string name)
        {
            this.cosmosClient = new CosmosClient(EndpointUri, PrimaryKey, cosmosClientOptions);
            
            var maxRetryAttempts = Int32.Parse(Environment.GetEnvironmentVariable("COSMOS_FAILURE_RETRIES"));  
            var pauseBetweenFailures = TimeSpan.FromSeconds(Int32.Parse(Environment.GetEnvironmentVariable("COSMOS_PAUSE_BETWEEN_FAILURES")));  

            await RetryHelper.RetryOnExceptionAsync<CosmosException>
                (maxRetryAttempts, pauseBetweenFailures, async () => {
                this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
            }); 

            await this.CreateContainerRootAsync();
            await this.CreateContainerTcxTrackpointAsync();

            QueryDefinition query = new QueryDefinition(
                "SELECT R.id, R.CreateDate FROM Root R");

            FeedIterator streamResultSet = containerRoot.GetItemQueryStreamIterator(
                query,
                requestOptions: new QueryRequestOptions()
                {
                    PartitionKey = new PartitionKey(name),
                    MaxItemCount = 10,
                    MaxConcurrency = 1
                });

            string rootId = "";

            while (streamResultSet.HasMoreResults)
            {
                using (ResponseMessage responseMessage = await streamResultSet.ReadNextAsync())
                {
                    responseMessage.EnsureSuccessStatusCode();
                    
                    using (StreamReader streamReader = new StreamReader(responseMessage.Content))
                    using (JsonTextReader jsonTextReader = new JsonTextReader(streamReader))
                    {
                        JObject jObject = await JObject.LoadAsync(jsonTextReader);
                        
                        JsonReader reader = jObject.CreateReader();
                        while (reader.Read())
                        {
                            JsonSerializer jsonSerializer = new JsonSerializer();
                            dynamic itemsRoot = jsonSerializer.Deserialize<dynamic>(reader).Documents;

                            for(int ii = 0; ii < itemsRoot.Count; ii++) {
                                dynamic itemRoot = itemsRoot[ii];
                                rootId = itemRoot.id;
                            }

                        }
                        
                    }
                }
            }   

            QueryDefinition queryTcxTrackpoint = new QueryDefinition(
                "SELECT  T.id, T.CreateDate, T.RootId, T.LapId, T.Time, T.TcxPosition, T.AltitudeMeters, T.DistanceMeters, T.TcxHeartRateBpm FROM Trackpoint T WHERE T.RootId = @RootIdInput ")
                .WithParameter("@RootIdInput", rootId);

            FeedIterator streamResultSetTcxTrackpoint = containerTcxTrackpoint.GetItemQueryStreamIterator(
                queryTcxTrackpoint,
                requestOptions: new QueryRequestOptions()
                {
                    PartitionKey = new PartitionKey(rootId),
                    MaxItemCount = 100,
                    MaxConcurrency = 1
                });

            List<TcxTrackpoint> trackpointList = new List<TcxTrackpoint>();

            while (streamResultSetTcxTrackpoint.HasMoreResults)
            {
                using (ResponseMessage responseMessageTcxTrackpoint = await streamResultSetTcxTrackpoint.ReadNextAsync())
                {
                    responseMessageTcxTrackpoint.EnsureSuccessStatusCode();
                    
                    using (StreamReader streamReaderTcxTrackpoint = new StreamReader(responseMessageTcxTrackpoint.Content))
                    using (JsonTextReader jsonTextReaderTcxTrackpoint = new JsonTextReader(streamReaderTcxTrackpoint))
                    {
                        JObject jObjectTcxTrackpoint = await JObject.LoadAsync(jsonTextReaderTcxTrackpoint);
                        
                        JsonReader readerTcxTrackpoint = jObjectTcxTrackpoint.CreateReader();
                        while (readerTcxTrackpoint.Read())
                        {
                            JsonSerializer jsonSerializerTcxTrackpoint = new JsonSerializer();
                            dynamic itemsTcxTrackpoint = jsonSerializerTcxTrackpoint.Deserialize<dynamic>(readerTcxTrackpoint).Documents;

                            for(int i = 0; i < itemsTcxTrackpoint.Count; i++) {
                                dynamic itemTcxTrackpoint = itemsTcxTrackpoint[i];
                                var trackpoint = CopyDynamic(itemTcxTrackpoint);
                                trackpointList.Add(trackpoint);
                            }

                        }
                        
                    }
                }
            }   

            return trackpointList;            
        }

        private async Task<List<Polylines>> GetDataRootPolylines (string lapId)
        {
            this.cosmosClient = new CosmosClient(EndpointUri, PrimaryKey, cosmosClientOptions);

            var maxRetryAttempts = Int32.Parse(Environment.GetEnvironmentVariable("COSMOS_FAILURE_RETRIES"));  
            var pauseBetweenFailures = TimeSpan.FromSeconds(Int32.Parse(Environment.GetEnvironmentVariable("COSMOS_PAUSE_BETWEEN_FAILURES")));  
            
            await RetryHelper.RetryOnExceptionAsync<CosmosException>
                (maxRetryAttempts, pauseBetweenFailures, async () => {
                this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
            }); 

            await this.CreateContainerRootAsync();
            await this.CreateContainerTcxTrackpointAsync();

            // TODO parameterize this value
            int polylinesLimit = 100000;
            
            QueryDefinition queryTcxTrackpoint = new QueryDefinition(
                "SELECT T.id, T.CreateDate, T.RootId, T.LapId, T.Time, T.TcxPosition, T.AltitudeMeters, T.DistanceMeters, T.DistanceKms, T.TcxHeartRateBpm, T.Speed, T.Fastest, T.FastestKm FROM Trackpoint T WHERE T.LapId = @LapIdInput ")
                .WithParameter("@LapIdInput", lapId);

            FeedIterator streamResultSetTcxTrackpoint = containerTcxTrackpoint.GetItemQueryStreamIterator(
                queryTcxTrackpoint,
                requestOptions: new QueryRequestOptions()
                );

            List<Polylines> polylinesList = new List<Polylines>();

            int polylinesProcessed = 0;

            while (streamResultSetTcxTrackpoint.HasMoreResults)
            {
                using (ResponseMessage responseMessageTcxTrackpoint = await streamResultSetTcxTrackpoint.ReadNextAsync())
                {
                    responseMessageTcxTrackpoint.EnsureSuccessStatusCode();
                    
                    using (StreamReader streamReaderTcxTrackpoint = new StreamReader(responseMessageTcxTrackpoint.Content))
                    using (JsonTextReader jsonTextReaderTcxTrackpoint = new JsonTextReader(streamReaderTcxTrackpoint))
                    {
                        JObject jObjectTcxTrackpoint = await JObject.LoadAsync(jsonTextReaderTcxTrackpoint);
                        
                        JsonReader readerTcxTrackpoint = jObjectTcxTrackpoint.CreateReader();
                        while (readerTcxTrackpoint.Read())
                        {
                            JsonSerializer jsonSerializerTcxTrackpoint = new JsonSerializer();
                            dynamic itemsTcxTrackpoint = jsonSerializerTcxTrackpoint.Deserialize<dynamic>(readerTcxTrackpoint).Documents;

                            for(int i = 0; i < itemsTcxTrackpoint.Count; i++) {
                                polylinesProcessed = polylinesProcessed + 1;
                                if (polylinesProcessed > polylinesLimit){
                                    continue;
                                }
                                dynamic itemTcxTrackpoint = itemsTcxTrackpoint[i];
                                if (i > 0){
                                    dynamic itemTcxTrackpointPrevious = itemsTcxTrackpoint[i-1];
                                    polylinesList = CopyDynamicPolylines(itemTcxTrackpoint, itemTcxTrackpointPrevious, i, polylinesList);
                                }
                                else {
                                    polylinesList = CopyDynamicPolylines(itemTcxTrackpoint, null, i, polylinesList);
                                }
                            }

                        }
                        
                    }
                }
            }   

            return polylinesList;            
        }

        
        private async Task<List<Lap>> GetDataRootLaps ()
        {
            this.cosmosClient = new CosmosClient(EndpointUri, PrimaryKey, cosmosClientOptions);
            
            var maxRetryAttempts = Int32.Parse(Environment.GetEnvironmentVariable("COSMOS_FAILURE_RETRIES"));  
            var pauseBetweenFailures = TimeSpan.FromSeconds(Int32.Parse(Environment.GetEnvironmentVariable("COSMOS_PAUSE_BETWEEN_FAILURES")));  
            
            await RetryHelper.RetryOnExceptionAsync<CosmosException>
                (maxRetryAttempts, pauseBetweenFailures, async () => {
                this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
            }); 

            await this.CreateContainerRootAsync();
            await this.CreateContainerLapAsync();

            QueryDefinition queryLap = new QueryDefinition(
                "SELECT L.id, L.RootId, L.CreateDate, L.TotalTimeSeconds, L.TotalTimeMinutes, L.DistanceMeters, L.DistanceKms, L.StartTime, L.Calories, L.FastestSpeed, L.FastestSpeedMarker, L.FastestKm, L.FastestKmMarker, L.HighestHr, L.HighestHrMarker, L.AverageHr FROM Lap L ORDER BY L.StartTime DESC");

            FeedIterator streamResultSetLap = containerLap.GetItemQueryStreamIterator(
                queryLap,
                requestOptions: new QueryRequestOptions()
                );

            List<Lap> lapsList = new List<Lap>();

            while (streamResultSetLap.HasMoreResults)
            {
                using (ResponseMessage responseMessageLap = await streamResultSetLap.ReadNextAsync())
                {
                    responseMessageLap.EnsureSuccessStatusCode();
                    
                    using (StreamReader streamReaderLap = new StreamReader(responseMessageLap.Content))
                    using (JsonTextReader jsonTextReaderLap = new JsonTextReader(streamReaderLap))
                    {

                        JObject jObjectLap = await JObject.LoadAsync(jsonTextReaderLap);
                        
                        JsonReader readerLap = jObjectLap.CreateReader();
                        while (readerLap.Read())
                        {
                            JsonSerializer jsonSerializerLap = new JsonSerializer();
                            dynamic itemsLap = jsonSerializerLap.Deserialize<dynamic>(readerLap).Documents;

                            for(int g = 0; g < itemsLap.Count; g++) {
                                dynamic itemLap = itemsLap[g];
                                lapsList = CopyDynamicLap(itemLap, lapsList);
                            }

                        }
                        
                    }
                }
            }   

            return lapsList;            
        }

        private async Task<string> GetDataRootInit ()
        {
            this.cosmosClient = new CosmosClient(EndpointUri, PrimaryKey, cosmosClientOptions);

            var maxRetryAttempts = Int32.Parse(Environment.GetEnvironmentVariable("COSMOS_FAILURE_RETRIES"));  
            var pauseBetweenFailures = TimeSpan.FromSeconds(Int32.Parse(Environment.GetEnvironmentVariable("COSMOS_PAUSE_BETWEEN_FAILURES")));  
            
            await RetryHelper.RetryOnExceptionAsync<CosmosException>
                (maxRetryAttempts, pauseBetweenFailures, async () => {
                this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
            }); 

            await this.CreateContainerRootAsync();

            QueryDefinition query = new QueryDefinition(
                "SELECT R.id, R.CreateDate FROM Root R");

            FeedIterator streamResultSet = containerRoot.GetItemQueryStreamIterator(
                query,
                requestOptions: new QueryRequestOptions()
            );

            string rootId = "";

            while (streamResultSet.HasMoreResults)
            {
                using (ResponseMessage responseMessage = await streamResultSet.ReadNextAsync())
                {
                    responseMessage.EnsureSuccessStatusCode();
                    
                    using (StreamReader streamReader = new StreamReader(responseMessage.Content))
                    using (JsonTextReader jsonTextReader = new JsonTextReader(streamReader))
                    {
                        JObject jObject = await JObject.LoadAsync(jsonTextReader);
                        
                        JsonReader reader = jObject.CreateReader();
                        while (reader.Read())
                        {
                            JsonSerializer jsonSerializer = new JsonSerializer();
                            dynamic itemsRoot = jsonSerializer.Deserialize<dynamic>(reader).Documents;

                            for(int ii = 0; ii < itemsRoot.Count; ii++) {
                                dynamic itemRoot = itemsRoot[ii];
                                rootId = itemRoot.id;
                            }

                        }
                    }
                }
            }   

            return rootId;
        }

        public static TcxTrackpoint CopyDynamic (dynamic dynamicTcx)
        {
            TcxTrackpoint tcxTrackpoint = new TcxTrackpoint();
            
            Guid newId = new Guid(dynamicTcx.id.ToString());
            tcxTrackpoint.Id = newId;
            
            Guid newLapId = Guid.Parse(dynamicTcx.LapId.ToString());
            tcxTrackpoint.LapId = newLapId;
            
            Guid newRootId = Guid.Parse(dynamicTcx.RootId.ToString());
            tcxTrackpoint.RootId = newRootId;

            tcxTrackpoint.CreateDate = dynamicTcx.CreateDate;
            tcxTrackpoint.TrackPointTime = dynamicTcx.Time;
            tcxTrackpoint.TcxPosition = (List<TcxPosition>)dynamicTcx.TcxPosition.ToObject(typeof(List<TcxPosition>));
            tcxTrackpoint.AltitudeMeters = dynamicTcx.AltitudeMeters;
            tcxTrackpoint.DistanceMeters = dynamicTcx.DistanceMeters;
            tcxTrackpoint.TcxHeartRateBpm = (List<TcxHeartRateBpm>)dynamicTcx.TcxHeartRateBpm.ToObject(typeof(List<TcxHeartRateBpm>));
            
            return tcxTrackpoint;
        }

        public static List<Polylines> CopyDynamicPolylines (dynamic dynamicTcx, dynamic dynamicTcxPrevious, int counter, List<Polylines> polylinesList)
        {

            string strokeColor = null;
            string previousStrokeColor = null;

            string forwardIcon = "FORWARD_CLOSED_ARROW";
            string backwardIcon = "BACKWARD_CLOSED_ARROW";
            string computedIcon = "FORWARD_CLOSED_ARROW";

            double speed = 0;

            var fastest = (bool)(bool)dynamicTcx.Fastest.ToObject(typeof(bool));
            var fastestKm = (bool)(bool)dynamicTcx.FastestKm.ToObject(typeof(bool));
            var tcxPositionList = (List<TcxPosition>)dynamicTcx.TcxPosition.ToObject(typeof(List<TcxPosition>));
            var tcxPositionListPrevious = dynamicTcxPrevious == null ? null : (List<TcxPosition>)dynamicTcxPrevious.TcxPosition.ToObject(typeof(List<TcxPosition>));

            if (!fastestKm){

                if (dynamicTcx.Speed != null) {
                    speed = dynamicTcx.Speed;
                    if (speed > 40){
                        strokeColor = "#68228b";
                    }
                    else if (speed > 35){
                        strokeColor = "#9932cc";
                    }
                    else if (speed > 30){
                        strokeColor = "#f00";
                    }
                    else if (speed > 20){
                        strokeColor = "#ff8c00";
                    }
                    else {
                        strokeColor = "#ffb90f";
                    }
                }
                else {
                    strokeColor = "#ffb90f";
                }

                if (dynamicTcxPrevious != null && dynamicTcxPrevious.Speed != null) {
                    double? previousSpeed = dynamicTcxPrevious.Speed;
                    if (previousSpeed > 40){
                        previousStrokeColor = "#68228b";
                    }
                    else if (previousSpeed > 35){
                        previousStrokeColor = "#9932cc";
                    }
                    else if (previousSpeed > 30){
                        previousStrokeColor = "#f00";
                    }
                    else if (previousSpeed > 20){
                        previousStrokeColor = "#ff8c00";
                    }
                    else {
                        previousStrokeColor = "#ffb90f";
                    }
                }
            }

            else if (fastestKm){
                
                if (dynamicTcx.Speed != null) {
                    speed = dynamicTcx.Speed;
                    if (speed > 40){
                        strokeColor = "#0f3d0f";
                    }
                    else if (speed > 35){
                        strokeColor = "#1f7a1f";
                    }
                    else if (speed > 30){
                        strokeColor = "#2eb82e";
                    }
                    else if (speed > 20){
                        strokeColor = "#5cd65c";
                    }
                    else {
                        strokeColor = "#99e699";
                    }
                }
                else {
                    strokeColor = "#99e699";
                }

                if (dynamicTcxPrevious != null && dynamicTcxPrevious.Speed != null) {
                    double? previousSpeed = dynamicTcxPrevious.Speed;
                    if (previousSpeed > 40){
                        previousStrokeColor = "#0f3d0f";
                    }
                    else if (previousSpeed > 35){
                        previousStrokeColor = "#1f7a1f";
                    }
                    else if (previousSpeed > 30){
                        previousStrokeColor = "#2eb82e";
                    }
                    else if (previousSpeed > 20){
                        previousStrokeColor = "#5cd65c";
                    }
                    else {
                        previousStrokeColor = "#99e699";
                    }
                }
            }

            bool createPaths = true;
            bool speedChanged = true;

            if (strokeColor != null && previousStrokeColor != null && strokeColor.Equals(previousStrokeColor) ) {
                speedChanged = false;
            }

            if (previousStrokeColor == null){
                speedChanged = true;
            }

            if (dynamicTcxPrevious != null){
                var fastestKmPrevious = (bool)(bool)dynamicTcxPrevious.FastestKm.ToObject(typeof(bool));
                if (fastestKm != fastestKmPrevious){
                    speedChanged = true;
                }
            }

            for(int i = 0; i < tcxPositionList.Count; i++) {

                dynamic tcxPosition = tcxPositionList[i];

                var currentLat = tcxPosition.LatitudeDegrees;

                if (tcxPositionListPrevious != null){

                    for(int ii = 0; ii < tcxPositionListPrevious.Count; ii++) {
                        dynamic tcxPositionPrevious = tcxPositionListPrevious[ii];
                        var previousLat = tcxPositionPrevious.LatitudeDegrees;

                        if (currentLat < previousLat) {
                            computedIcon = backwardIcon;
                        }
                        else {
                            computedIcon = forwardIcon;
                        }
                    }
                }
            }

            if (speedChanged.Equals(false) || fastest.Equals(true) || fastestKm.Equals(true)){
                
                for(int a = 0; a < polylinesList.Count; a++) {
                    
                    Polylines polylines = polylinesList[a];
                        
                    Paths paths = polylines.Paths.FindLast(p => p.Color.Equals(strokeColor));

                    List<Paths> listPaths = polylines.Paths;

                    if (paths != null){

                        createPaths = false;

                        List<Points> pointsList = paths.Points;
                
                        for(int b = 0; b < tcxPositionList.Count; b++) {

                            dynamic tcxPosition = tcxPositionList[b];
                            Points points = new Points();
                            points.Latitude = tcxPosition.LatitudeDegrees;
                            points.Longitude = tcxPosition.LongitudeDegrees;
                            points.Fastest = fastest;

                            if (points.Fastest){
                                if (speed > 40){
                                    points.Icon = "https://maps.google.com/mapfiles/ms/icons/purple.png";
                                }
                                else if (speed > 35){
                                    points.Icon = "https://maps.google.com/mapfiles/ms/icons/red.png";
                                }
                                else if (speed > 30){
                                    points.Icon = "https://maps.google.com/mapfiles/ms/icons/orange.png";
                                }
                                else if (speed > 20){
                                    points.Icon = "https://maps.google.com/mapfiles/ms/icons/yellow.png";
                                }
                                else {
                                    points.Icon = "https://maps.google.com/mapfiles/ms/icons/blue.png";
                                }
                                points.Speed = Math.Round(speed, 2).ToString();
                            }

                            pointsList.Add(points);
                        
                            Paths listIndexPaths = listPaths.Where(d=> d.Id == paths.Id).First();
                            var indexPaths = listPaths.IndexOf(listIndexPaths);

                            paths.Points = pointsList;

                            if(indexPaths != -1)
                                listPaths[indexPaths] = paths;

                            polylines.Paths = listPaths;

                            Polylines listIndexPolylines= polylinesList.Where(e=> e.Id == polylines.Id).First();
                            var indexPolylines = polylinesList.IndexOf(listIndexPolylines);

                            if(indexPolylines != -1)
                                polylinesList[indexPolylines] = polylines;
                        }
                    }
                }

            }

            if (createPaths.Equals(false) && speedChanged.Equals(false)){
                return polylinesList;
            }

            if (createPaths.Equals(true) || speedChanged.Equals(true)){

                if (polylinesList.Count.Equals(0)){
                    Polylines polylines = new Polylines();
                    Guid newId = Guid.NewGuid();
                    polylines.Id = newId;
                    polylinesList.Add(polylines);
                }

                for(int a = 0; a < polylinesList.Count; a++) {
                
                    Polylines polylines = polylinesList[a];

                    List<Paths> pathsList = new List<Paths>();
                    if (polylines.Paths != null){
                        pathsList = polylines.Paths;
                    }
                    List<Points> pointsList = new List<Points>();
                    
                    for(int bb = 0; bb < tcxPositionList.Count; bb++) {
                        dynamic tcxPosition = tcxPositionList[bb];

                        Paths paths = new Paths();
                        Guid newPathId = Guid.NewGuid();
                        paths.Id = newPathId;
                        paths.Color = strokeColor;
                        paths.Icon = computedIcon;

                        if (tcxPositionListPrevious != null){

                            for(int bbb = 0; bbb < tcxPositionListPrevious.Count; bbb++) {

                                dynamic tcxPositionPrevious = tcxPositionListPrevious[bbb];

                                Points pointsPrevious = new Points();
                                pointsPrevious.Latitude = tcxPositionPrevious.LatitudeDegrees;
                                pointsPrevious.Longitude = tcxPositionPrevious.LongitudeDegrees;
                                
                                Points points = new Points();
                                points.Latitude = tcxPosition.LatitudeDegrees;
                                points.Longitude = tcxPosition.LongitudeDegrees;
                                points.Fastest = fastest;

                                if (points.Fastest){
                                    if (speed > 40){
                                        points.Icon = "https://maps.google.com/mapfiles/ms/icons/purple.png";
                                    }
                                    else if (speed > 35){
                                        points.Icon = "https://maps.google.com/mapfiles/ms/icons/red.png";
                                    }
                                    else if (speed > 30){
                                        points.Icon = "https://maps.google.com/mapfiles/ms/icons/orange.png";
                                    }
                                    else if (speed > 20){
                                        points.Icon = "https://maps.google.com/mapfiles/ms/icons/yellow.png";
                                    }
                                    else {
                                        points.Icon = "https://maps.google.com/mapfiles/ms/icons/blue.png";
                                    }
                                    points.Speed = Math.Round(speed, 2).ToString();
                                }

                                pointsList.Add(pointsPrevious);
                                pointsList.Add(points);
                                paths.Points = pointsList;

                                pathsList.Add(paths);

                                polylines.Paths = pathsList;
                            }
                        }
                        else {
                                
                            Points points = new Points();
                            points.Latitude = tcxPosition.LatitudeDegrees;
                            points.Longitude = tcxPosition.LongitudeDegrees;
                            points.Fastest = fastest;

                            if (points.Fastest){
                                if (speed > 40){
                                    points.Icon = "https://maps.google.com/mapfiles/ms/icons/purple.png";
                                }
                                else if (speed > 35){
                                    points.Icon = "https://maps.google.com/mapfiles/ms/icons/red.png";
                                }
                                else if (speed > 30){
                                    points.Icon = "https://maps.google.com/mapfiles/ms/icons/orange.png";
                                }
                                else if (speed > 20){
                                    points.Icon = "https://maps.google.com/mapfiles/ms/icons/yellow.png";
                                }
                                else {
                                    points.Icon = "https://maps.google.com/mapfiles/ms/icons/blue.png";
                                }
                                points.Speed = Math.Round(speed, 2).ToString();
                            }

                            pointsList.Add(points);
                            paths.Points = pointsList;

                            pathsList.Add(paths);

                            polylines.Paths = pathsList;
                        }
                    }

                    Polylines listIndexPolylines= polylinesList.Where(e=> e.Id == polylines.Id).First();
                    var indexPolylines = polylinesList.IndexOf(listIndexPolylines);

                    if(indexPolylines != -1)
                        polylinesList[indexPolylines] = polylines;

                }

                return polylinesList;
            }

            return polylinesList;
        }

        public static List<Lap> CopyDynamicLap (dynamic dynamicLap, List<Lap> lapsList)
        {
            Lap lap = new Lap();
            
            Guid newId = new Guid(dynamicLap.id.ToString());
            lap.Id = newId;
            
            Guid newRootId = Guid.Parse(dynamicLap.RootId.ToString());
            lap.RootId = newRootId;

            lap.CreateDate = dynamicLap.CreateDate;
            lap.StartTime = dynamicLap.StartTime;
            lap.Calories = dynamicLap.Calories;
            lap.TotalTimeSeconds = dynamicLap.TotalTimeSeconds;
            lap.TotalTimeMinutes = dynamicLap.TotalTimeMinutes;
            lap.DistanceMeters = dynamicLap.DistanceMeters;
            lap.DistanceKms = dynamicLap.DistanceKms;
            lap.FastestSpeed = dynamicLap.FastestSpeed;
            lap.FastestSpeedMarker = dynamicLap.FastestSpeedMarker;
            lap.FastestKm = dynamicLap.FastestKm;
            lap.FastestKmMarker = dynamicLap.FastestKmMarker;
            lap.HighestHr = dynamicLap.HighestHr;
            lap.HighestHrMarker = dynamicLap.HighestHrMarker;
            lap.AverageHr = dynamicLap.AverageHr;
            
            lap.DistanceKms = Math.Round(lap.DistanceKms, 1);
            lap.FastestSpeed = Math.Round(lap.FastestSpeed, 1);
            lap.FastestSpeedMarker = Math.Round(lap.FastestSpeedMarker/1000, 0);
            lap.FastestKm = Math.Round(lap.FastestKm, 1);
            lap.FastestKmMarker = Math.Round(lap.FastestKmMarker/1000, 0);
            lap.HighestHrMarker = Math.Round(lap.HighestHrMarker/1000, 0);
            lap.AverageHr = Math.Round(lap.AverageHr, 1);
            lap.TotalTimeMinutes = Math.Round(lap.TotalTimeMinutes, 1);

            lap.DropDown = lap.StartTime.Substring(0, 10) + " " + String.Format("{0:0}", lap.DistanceKms);

            lapsList.Add(lap);

            return lapsList;

        }

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        { 
            Dispose(true);
            GC.SuppressFinalize(this);           
        }
   
        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return; 
            
            if (disposing) {
                // Free any other managed objects here.
                //
            }
            
            // Free any unmanaged objects here.
            //
            disposed = true;
        }

        ~FitbitCosmos()
        {
            Dispose(false);
        }

    }

}