define(["sitecore", "/-/speak/v1/experienceprofile/CintelUtl.js", "../Social/SocialUtil.js"], function (sc, cintelUtil, socialUtil) {
    var app = sc.Definitions.App.extend({
        initialized: function () {
            var contactId = cintelUtil.getQueryParam("cid");

            var that = this;

            var fitbitDataService = socialUtil.getEntityService("/sitecore/api/ssc/sitecore-social-fitbit-experienceprofile/fitbitdata"); 
            fitbitDataService.fetchEntity(contactId).execute().then(function (fitbitData) {

                that.NoFitbitDataPanel.set("isVisible", false);
                that.FitbitDataPanel.set("isVisible", true);

                that.FitbitDataAvatar.set("imageUrl", fitbitData.Avatar);

                cintelUtil.setText(that.FitbitDataFullNameValue, fitbitData.FullName, false);
                cintelUtil.setText(that.FitbitDataGenderValue, fitbitData.Gender, false);
                cintelUtil.setText(that.FitbitDataCityValue, fitbitData.City, false);
                cintelUtil.setText(that.FitbitDataStateValue, fitbitData.State, false);
                cintelUtil.setText(that.FitbitDataCountryValue, fitbitData.Country, false);
                cintelUtil.setText(that.FitbitDataHeightValue, fitbitData.Height, false);
                cintelUtil.setText(that.FitbitDataHeightUnitValue, fitbitData.HeightUnit, false);
                cintelUtil.setText(that.FitbitDataWeightValue, fitbitData.Weight, false);
                cintelUtil.setText(that.FitbitDataWeightUnitValue, fitbitData.WeightUnit, false);
                cintelUtil.setText(that.FitbitDataAboutMeValue, fitbitData.AboutMe, false);


            }).fail(function (error) {

                that.NoFitbitDataPanel.set("isVisible", true);
                that.FitbitDataPanel.set("isVisible", false);

            });
        }
    });

    return app;
});