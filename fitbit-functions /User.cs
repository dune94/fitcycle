using Newtonsoft.Json;

namespace Fitbit {
    
    public class JsonUser
    {
        [JsonProperty(PropertyName = "user")]
        public User User { get; set; }
    }

    public class User
    {
        [JsonProperty(PropertyName = "firstName")]
        public string FirstName { get; set; }
        [JsonProperty(PropertyName = "lastName")]
        public string LastName { get; set; }
        [JsonProperty(PropertyName = "fullName")]
        public string FullName { get; set; }
        [JsonProperty(PropertyName = "displayName")]
        public string DisplayName { get; set; }
         [JsonProperty(PropertyName = "age")]
        public double Age { get; set; }
        [JsonProperty(PropertyName = "weight")]
        public double Weight { get; set; }
        [JsonProperty(PropertyName = "weightUnit")]
        public string WeightUnit { get; set; }
    }

}