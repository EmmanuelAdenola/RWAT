using System.Web.Mvc;

namespace RWAT.Controllers
{
    public class NavigationController : Controller
    {
        [ChildActionOnly]
        public ActionResult Index()
        {
                return View("AuthenticatedNav");
        }

    }
}
