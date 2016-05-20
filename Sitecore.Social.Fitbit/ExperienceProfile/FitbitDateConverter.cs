using Sitecore.Social.ExperienceProfile.Client.Repositories.Converters;

namespace Sitecore.Social.Fitbit.ExperienceProfile
{
    public class FitbitDateConverter : NetworkDateConverter
    {
        public override string GetFormattedDate(string encodedDate, string culture)
        {
            if (string.IsNullOrEmpty(encodedDate))
            {
                return string.Empty;
            }

            return new DateConverter().GetFormattedDate(encodedDate.Substring(0, 4), encodedDate.Substring(5, 2), encodedDate.Substring(8, 2), culture);
        }
    }
}
