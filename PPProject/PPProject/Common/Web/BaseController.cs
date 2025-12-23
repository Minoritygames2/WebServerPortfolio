using Microsoft.AspNetCore.Mvc;
using PPProject.Common.Extensions;

namespace PPProject.Common.Web
{
    public abstract class BaseController : ControllerBase
    {
        protected long UserId => HttpContext.GetUserId();
    }
}
