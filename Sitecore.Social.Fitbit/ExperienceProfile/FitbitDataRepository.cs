using Sitecore.Social.Domain;
using Sitecore.Social.Domain.Model;
using Sitecore.Social.ExperienceProfile.Client.Repositories;
using Sitecore.Social.ExperienceProfile.Client.Repositories.Converters;
using Sitecore.Social.ExperienceProfile.Client.Repositories.Extensions;

namespace Sitecore.Social.Fitbit.ExperienceProfile
{
    public class FitbitDataRepository : SocialDataRepository<FitbitData>
    {
        public FitbitDataRepository()
            : base("Fitbit", new FitbitDateConverter())
        {
        }

        public FitbitDataRepository(ISocialProfileManager socialProfileManager, INetworkDateConverter networkDateConverter)
            : base("Fitbit", socialProfileManager, networkDateConverter)
        {
        }

        protected override void FillSocialData(FitbitData socialData, SocialProfile socialProfile)
        {
            socialData.AboutMe = socialProfile.ExtractField("ft_aboutMe");
            socialData.Avatar = socialProfile.ExtractField("ft_avatar");
            socialData.Avatar150 = socialProfile.ExtractField("ft_avatar150");
            socialData.City = socialProfile.ExtractField("ft_city");
            socialData.Country = socialProfile.ExtractField("ft_country");
            socialData.DateOfBirth = NetworkDateConverter.GetFormattedDate(socialProfile.ExtractField("ft_dateOfBirth"), Context.Language.CultureInfo.ToString());
            socialData.DisplayName = socialProfile.ExtractField("ft_displayName");
            socialData.DistanceUnit = socialProfile.ExtractField("ft_distanceUnit");
            socialData.EncodedId = socialProfile.ExtractField("ft_encodedId");
            socialData.FoodsLocale = socialProfile.ExtractField("ft_foodsLocale");
            socialData.FullName = socialProfile.ExtractField("ft_fullName");
            socialData.Gender = socialProfile.ExtractField("ft_gender");
            socialData.GlucoseUnit = socialProfile.ExtractField("ft_glucoseUnit");
            socialData.Height = socialProfile.ExtractField("ft_height");
            socialData.HeightUnit = socialProfile.ExtractField("ft_heightUnit");
            socialData.Locale = socialProfile.ExtractField("ft_locale");
            socialData.MemberSince = NetworkDateConverter.GetFormattedDate(socialProfile.ExtractField("ft_memberSince"), Context.Language.CultureInfo.ToString());
            socialData.Nickname = socialProfile.ExtractField("ft_nickname");
            socialData.OffsetFromUTCMillis = socialProfile.ExtractField("ft_offsetFromUTCMillis");
            socialData.StartDayOfWeek = socialProfile.ExtractField("ft_startDayOfWeek");
            socialData.State = socialProfile.ExtractField("ft_state");
            socialData.StrideLengthRunning = socialProfile.ExtractField("ft_strideLengthRunning");
            socialData.StrideLengthWalking = socialProfile.ExtractField("ft_strideLengthWalking");
            socialData.Timezone = socialProfile.ExtractField("ft_timezone");
            socialData.VolumeUnit = socialProfile.ExtractField("ft_volumeUnit");
            socialData.WaterUnit = socialProfile.ExtractField("ft_waterUnit");
            socialData.Weight = socialProfile.ExtractField("ft_weight");
            socialData.WeightUnit = socialProfile.ExtractField("ft_weightUnit");
        }
    }
}
