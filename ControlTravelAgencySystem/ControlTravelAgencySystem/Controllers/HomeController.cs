using System.Web.Mvc;

namespace ControlTravelAgencySystem.Controllers
{
    /// <summary>
    /// Контроллер домашней страницы
    /// </summary>
    public class HomeController : Controller
    {
        /// <summary>
        /// GET: /
        /// </summary>
        public ActionResult Index()
        {
            return View();
        }
    }
}
