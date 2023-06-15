using Microsoft.AspNetCore.Mvc;

namespace IhorsBookWeb.Areas.Customer.Controllers
{
    public class CookiePolicyController : Controller
    {
        public IActionResult Index()
        {
            DateTime dateTime = DateTime.Now.Date;
            ViewBag.Date = dateTime;
            return View();
        }
    }
}
