using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Fitbit {
    public class MinutesInHeartRateZone
    {
        [JsonProperty(PropertyName = "minuteMultiplier")]
        public int MinuteMultiplier { get; set; }
        [JsonProperty(PropertyName = "minutes")]
        public int Minutes { get; set; }
        [JsonProperty(PropertyName = "order")]
        public int Order { get; set; }
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
        [JsonProperty(PropertyName = "zoneName")]
        public string ZoneName { get; set; }
    }

    public class ActiveZoneMinutes
    {
        [JsonProperty(PropertyName = "minutesInHeartRateZones")]
        public List<MinutesInHeartRateZone> MinutesInHeartRateZones { get; set; }
        [JsonProperty(PropertyName = "totalMinutes")]
        public int TotalMinutes { get; set; }
    }

    public class ActivityLevel
    {
        [JsonProperty(PropertyName = "minutes")]
        public int Minutes { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }

    public class CustomHeartRateZone
    {
        [JsonProperty(PropertyName = "max")]
        public int Max { get; set; }
        [JsonProperty(PropertyName = "min")]
        public int Min { get; set; }
        [JsonProperty(PropertyName = "minutes")]
        public int Minutes { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }

    public class HeartRateZone
    {
        [JsonProperty(PropertyName = "caloriesOut")]
        public double CaloriesOut { get; set; }
        [JsonProperty(PropertyName = "max")]
        public int Max { get; set; }
        [JsonProperty(PropertyName = "min")]
        public int Min { get; set; }
        [JsonProperty(PropertyName = "minutes")]
        public int Minutes { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }

    public class ManualValuesSpecified
    {
        [JsonProperty(PropertyName = "calories")]
        public bool Calories { get; set; }
        [JsonProperty(PropertyName = "distance")]
        public bool Distance { get; set; }
        [JsonProperty(PropertyName = "steps")]
        public bool Steps { get; set; }
    }

    public class Source
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "trackerFeatures")]
        public List<string> TrackerFeatures { get; set; }
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }
    }

    public class Activity
    {
        [JsonProperty(PropertyName = "activeDuration")]
        public int ActiveDuration { get; set; }
        [JsonProperty(PropertyName = "activeZoneMinutes")]
        public ActiveZoneMinutes ActiveZoneMinutes { get; set; }
        [JsonProperty(PropertyName = "activityLevel")]
        public List<ActivityLevel> ActivityLevel { get; set; }
        [JsonProperty(PropertyName = "activityName")]
        public string ActivityName { get; set; }
        [JsonProperty(PropertyName = "activityTypeId")]
        public int ActivityTypeId { get; set; }
        [JsonProperty(PropertyName = "averageHeartRate")]
        public int AverageHeartRate { get; set; }
        [JsonProperty(PropertyName = "calories")]
        public int Calories { get; set; }
        [JsonProperty(PropertyName = "customHeartRateZones")]
        public List<CustomHeartRateZone> CustomHeartRateZones { get; set; }
        [JsonProperty(PropertyName = "duration")]
        public int Duration { get; set; }
        [JsonProperty(PropertyName = "elevationGain")]
        public double ElevationGain { get; set; }
        [JsonProperty(PropertyName = "hasActiveZoneMinutes")]
        public bool HasActiveZoneMinutes { get; set; }
        [JsonProperty(PropertyName = "heartRateLink")]
        public string HeartRateLink { get; set; }
        [JsonProperty(PropertyName = "heartRateZones")]
        public List<HeartRateZone> HeartRateZones { get; set; }
        [JsonProperty(PropertyName = "lastModified")]
        public DateTime LastModified { get; set; }
        [JsonProperty(PropertyName = "logId")]
        public object LogId { get; set; }
        [JsonProperty(PropertyName = "logType")]
        public string LogType { get; set; }
        [JsonProperty(PropertyName = "manualValuesSpecified")]
        public ManualValuesSpecified ManualValuesSpecified { get; set; }
        [JsonProperty(PropertyName = "originalDuration")]
        public int OriginalDuration { get; set; }
        [JsonProperty(PropertyName = "originalStartTime")]
        public DateTime OriginalStartTime { get; set; }
        [JsonProperty(PropertyName = "startTime")]
        public DateTime StartTime { get; set; }
        [JsonProperty(PropertyName = "steps")]
        public int Steps { get; set; }
        [JsonProperty(PropertyName = "tcxLink")]
        public string TcxLink { get; set; }
        [JsonProperty(PropertyName = "distance")]
        public double? Distance { get; set; }
        [JsonProperty(PropertyName = "distanceUnit")]
        public string DistanceUnit { get; set; }
        [JsonProperty(PropertyName = "pace")]
        public double? Pace { get; set; }
        [JsonProperty(PropertyName = "source")]
        public Source Source { get; set; }
        [JsonProperty(PropertyName = "speed")]
        public double? Speed { get; set; }
        [JsonProperty(PropertyName = "watts")]
        public double? Watts { get; set; }
    }

    public class Pagination
    {
        [JsonProperty(PropertyName = "afterDate")]
        public string AfterDate { get; set; }
        [JsonProperty(PropertyName = "limit")]
        public int Limit { get; set; }
        [JsonProperty(PropertyName = "next")]
        public string Next { get; set; }
        [JsonProperty(PropertyName = "offset")]
        public int Offset { get; set; }
        [JsonProperty(PropertyName = "previous")]
        public string Previous { get; set; }
        [JsonProperty(PropertyName = "sort")]
        public string Sort { get; set; }
    }

    public class Root
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }
        [JsonProperty(PropertyName = "CreateDate")]
        public string CreateDate { get; set; }
        [JsonProperty(PropertyName = "activities")]
        public List<Activity> Activities { get; set; }
        [JsonProperty(PropertyName = "pagination")]
        public Pagination Pagination { get; set; }
    }

    [XmlRoot(ElementName="TrainingCenterDatabase", Namespace="http://www.garmin.com/xmlschemas/TrainingCenterDatabase/v2")]
    public class TcxTrainingCenterDatabase
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }
        [XmlElement("Activities"), JsonProperty(PropertyName = "Activities")]
        public List<TcxActivities> TcxActivities { get; set; }
    }

    public class TcxActivities
    {
        [XmlElementAttribute("Activity"), JsonProperty(PropertyName = "Activity")]
        public List<TcxActivity> TcxActivity { get; set; }
    }

    public class TcxActivity
    {
        [XmlAttribute("Sport"), JsonProperty(PropertyName = "Sport")]
        public string Sport { get; set; }
        [XmlElement("Id"), JsonProperty(PropertyName = "Id")]
        public string TcxId { get; set; }
        [XmlElement("Lap"), JsonProperty(PropertyName = "Lap")]
        public List<TcxLap> TcxLap { get; set; }
    }

    public class TcxLap
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }
        [XmlAttribute("StartTime"), JsonProperty(PropertyName = "StartTime")]
        public string StartTime { get; set; }
        [XmlElement("TotalTimeSeconds"), JsonProperty(PropertyName = "TotalTimeSeconds")]
        public double TotalTimeSeconds { get; set; }
        [XmlElement("DistanceMeters"), JsonProperty(PropertyName = "DistanceMeters")]
        public double DistanceMeters { get; set; }
        [XmlElement("Calories"), JsonProperty(PropertyName = "Calories")]
        public int Calories { get; set; }
        [XmlElement("Intensity"), JsonProperty(PropertyName = "Intensity")]
        public string Intensity { get; set; }
        [XmlElement("TriggerMethod"), JsonProperty(PropertyName = "TriggerMethod")]
        public string TriggerMethod { get; set; }
        [XmlElement("Track"), JsonProperty(PropertyName = "Track")]
        public List<TcxTrack> TcxTrack { get; set; }
    }

    public class Lap
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }
        [JsonProperty(PropertyName = "RootId")]
        public Guid RootId { get; set; }
        [JsonProperty(PropertyName = "CreateDate")]
        public string CreateDate { get; set; }
        [JsonProperty(PropertyName = "StartTime")]
        public string StartTime { get; set; }
        [JsonProperty(PropertyName = "TotalTimeSeconds")]
        public double TotalTimeSeconds { get; set; }
        [JsonProperty(PropertyName = "TotalTimeMinutes")]
        public double TotalTimeMinutes { get; set; }
        [JsonProperty(PropertyName = "DistanceMeters")]
        public double DistanceMeters { get; set; }
        [JsonProperty(PropertyName = "DistanceKms")]
        public double DistanceKms { get; set; }
        [JsonProperty(PropertyName = "Calories")]
        public int Calories { get; set; }
        [JsonProperty(PropertyName = "Watts")]
        public double Watts { get; set; }
        [JsonProperty(PropertyName = "WattsPerKg")]
        public double WattsPerKg { get; set; }
        [JsonProperty(PropertyName = "Intensity")]
        public string Intensity { get; set; }
        [JsonProperty(PropertyName = "TriggerMethod")]
        public string TriggerMethod { get; set; }
        [JsonProperty(PropertyName = "DropDown")]
        public string DropDown { get; set; }
        [JsonProperty(PropertyName = "Activity")]
        public string Activity { get; set; }
        [JsonProperty(PropertyName = "FastestSpeed")]
        public double FastestSpeed { get; set; }
        [JsonProperty(PropertyName = "FastestSpeedMarker")]
        public double FastestSpeedMarker { get; set; }
        [JsonProperty(PropertyName = "FastestKm")]
        public double FastestKm { get; set; }
        [JsonProperty(PropertyName = "FastestKmMarker")]
        public double FastestKmMarker { get; set; }
        [JsonProperty(PropertyName = "FastestMeters")]
        public double FastestMeters { get; set; }
        [JsonProperty(PropertyName = "HighestHr")]
        public double HighestHr { get; set; }
        [JsonProperty(PropertyName = "HighestHrMarker")]
        public double HighestHrMarker { get; set; }
        [JsonProperty(PropertyName = "HighestHrMeters")]
        public double HighestHrMeters { get; set; }
        [JsonProperty(PropertyName = "AverageHr")]
        public double AverageHr { get; set; }
        [JsonProperty(PropertyName = "AverageSpeed")]
        public double AverageSpeed { get; set; }
        [JsonProperty(PropertyName = "HighestWattsMarker")]
        public double HighestWattsMarker { get; set; }
        [JsonProperty(PropertyName = "HighestWatts")]
        public double HighestWatts { get; set; }
    }

    public class TcxTrack
    {
        [XmlElement("Trackpoint"), JsonProperty(PropertyName = "Trackpoint")]
        public List<TcxTrackpoint> TcxTrackpoint { get; set; }
    }

    public class TcxTrackpoint
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }
        [JsonProperty(PropertyName = "LapId")]
        public Guid LapId { get; set; }
        [JsonProperty(PropertyName = "RootId")]
        public Guid RootId { get; set; }
        [JsonProperty(PropertyName = "CreateDate")]
        public string CreateDate { get; set; }
        [XmlElement("Time"), JsonProperty(PropertyName = "Time")]
        public string TrackPointTime { get; set; }
        [XmlElement("Position")]
        public List<TcxPosition> TcxPosition { get; set; }
        [XmlElement("AltitudeMeters"), JsonProperty(PropertyName = "AltitudeMeters")]
        public double AltitudeMeters { get; set; }
        [XmlElement("DistanceMeters"), JsonProperty(PropertyName = "DistanceMeters")]
        public double DistanceMeters { get; set; }
        [XmlElement("HeartRateBpm")]
        public List<TcxHeartRateBpm> TcxHeartRateBpm { get; set; }
        public double Speed { get; set; }
        public double Watts { get; set; }
        public double HeartRate { get; set; }
        public double Calories { get; set; }
        public double TotalTimeSeconds { get; set; }
        public bool Fastest { get; set; }
        public bool FastestKm { get; set; }
        public bool HighestHr { get; set; }
        public bool HighestCalory { get; set; }
    }

    public class TcxPosition
    {
        [XmlElement("LatitudeDegrees")]
        public double LatitudeDegrees { get; set; }
        [XmlElement("LongitudeDegrees")]
        public double LongitudeDegrees { get; set; }
    }

    public class TcxHeartRateBpm
    {
        [XmlElement("Value")]
        public int HeartRateBpmValue { get; set; }
    }

    public class RenderOptions 
    {
        [JsonProperty("polylineOptions")]
        public PolylineOptions PolylineOptions { get; set; }
    }

    public class PolylineOptions 
    {
        [JsonProperty("strokeColor")]
        public string StrokeColor { get; set; }
    }

    public class Polylines 
    {
        public Guid Id { get; set; }
        [JsonProperty("speedpaths")]
        public List<Paths> SpeedPaths { get; set; }
        [JsonProperty("hrpaths")]
        public List<Paths> HrPaths { get; set; }
        [JsonProperty("calorypaths")]
        public List<Paths> CaloryPaths { get; set; }
    }

    public class Paths
    {
        public Guid Id { get; set; }
        [JsonProperty("point")]
        public List<Points> Points { get; set; }
        [JsonProperty("color")]
        public string Color { get; set; }
        [JsonProperty("icon")]
        public string Icon { get; set; }
    }

    public class Points
    {
        [JsonProperty("latitude")]
        public double Latitude { get; set; }
        [JsonProperty("longitude")]
        public double Longitude { get; set; }
        [JsonProperty("fastest")]
        public bool Fastest { get; set; }
        [JsonProperty("highesthr")]
        public bool HighestHr { get; set; }
        [JsonProperty("speed")]
        public string Speed { get; set; }
        [JsonProperty("heartrate")]
        public string HeartRate { get; set; }
        public string Calories { get; set; }
        public string Watts { get; set; }
        [JsonProperty("icon")]
        public string Icon { get; set; }
    }

    public class Load
    {
        [JsonProperty("dataExists")]
        public string DataExists { get; set; }
    }

}