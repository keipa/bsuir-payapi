using System.Web.Mvc;

namespace PayAPI.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "PayAPI";

            return View();
        }
    }
}