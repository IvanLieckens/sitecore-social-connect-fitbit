using System.Web.Http;

using Sitecore.Services.Core;
using Sitecore.Services.Infrastructure.Sitecore.Services;
using Sitecore.Social.ExperienceProfile.Client.Controllers.ActionFilters;
using Sitecore.Web.Http.Filters;

namespace Sitecore.Social.Fitbit.ExperienceProfile
{
    [ValidateHttpAntiForgeryToken]
    [ApplyShellContext]
    [ServicesController]
    [Authorize]
    public class FitbitDataController : EntityService<FitbitData>
    {
        public FitbitDataController()
            : this(new FitbitDataRepository())
        {
        }

        public FitbitDataController(IRepository<FitbitData> repository)
            : base(repository)
        {
        }
    }
}
