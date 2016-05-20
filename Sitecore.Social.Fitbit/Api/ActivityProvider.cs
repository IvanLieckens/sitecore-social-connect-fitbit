using Sitecore.Social.Fitbit.Helpers;
using Sitecore.Social.Fitbit.Model;

namespace Sitecore.Social.Fitbit.Api
{
    public class ActivityProvider
    {
        private static string _dailySummaryUrlFormat = "https://api.fitbitcom/1/user/{0}/activities/date/{1}.json";

        private static string _lifetimeUrlFormat = "https://api.fitbit.com/1/user/{0}/activities.json";

        public static string DailySummaryUrlFormat
        {
            get
            {
                return _dailySummaryUrlFormat;
            }

            set
            {
                _dailySummaryUrlFormat = value;
            }
        }

        public static string LifetimeUrlFormat
        {
            get
            {
                return _lifetimeUrlFormat;
            }

            set
            {
                _lifetimeUrlFormat = value;
            }
        }

        public static LifetimeResponse GetCurrentUserLifetime(string token)
        {
            string apiUrl = string.Format(LifetimeUrlFormat, "-");
            LifetimeResponse result = RequestHelper.ExecuteRequest<LifetimeResponse>(apiUrl, token);

            return result;
        }
    }
}
