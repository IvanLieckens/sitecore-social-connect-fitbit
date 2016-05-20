using System.Web.Mvc;

using Sitecore.Globalization;
using Sitecore.Social.Client.Mvc.Areas.Social.Controllers;

namespace Sitecore.Social.Fitbit.Client.Mvc.Areas.Social.Controllers
{
    public class FitbitConnectorController : ConnectorController
    {
        public FitbitConnectorController()
            : base("Fitbit", Common.ItemIDs.LoginWithFitbitImageId)
        {
        }

        public ActionResult Index()
        {
            string tooltip = Translate.Text(Context.User.IsAuthenticated ? Common.Texts.AttachFitbitAccount : Common.Texts.LoginWithFitbit);
            return LoginPartialView(tooltip);
        }
    }
}
