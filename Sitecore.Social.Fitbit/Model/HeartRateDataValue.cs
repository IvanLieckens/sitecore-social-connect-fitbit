using System.Collections.Generic;

namespace Sitecore.Social.Fitbit.Model
{
    public class HeartRateDataValue
    {
        public List<HeartRateZone> CustomHeartRateZones { get; set; }

        public List<HeartRateZone> HeartRateZones { get; set; }

        public int RestingHeartRate { get; set; }
    }
}
