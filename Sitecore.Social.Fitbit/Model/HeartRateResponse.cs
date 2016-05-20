using System.Collections.Generic;

using Newtonsoft.Json;

namespace Sitecore.Social.Fitbit.Model
{
    public class HeartRateResponse
    {
        [JsonProperty("activities-heart")]
        public List<HeartRateData> ActivitiesHeart { get; set; }
    }
}
