using System;

using Sitecore.Social.Fitbit.Helpers;
using Sitecore.Social.Fitbit.Model;

namespace Sitecore.Social.Fitbit.Api
{
    public class HeartRateProvider
    {
        private static string _heartRateUrlFormat = "https://api.fitbit.com/1/user/{0}/activities/heart/date/{1}/{2}.json";
        
        public static string HeartRateUrlFormat
        {
            get
            {
                return _heartRateUrlFormat;
            }

            set
            {
                _heartRateUrlFormat = value;
            }
        }

        public static HeartRateResponse GetCurrentUserHeartRate(string token, DateTime startDate, DateTime endDate)
        {
            string apiUrl = string.Format(HeartRateUrlFormat, "-", startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));
            HeartRateResponse result = RequestHelper.ExecuteRequest<HeartRateResponse>(apiUrl, token);

            return result;
        }
    }
}
