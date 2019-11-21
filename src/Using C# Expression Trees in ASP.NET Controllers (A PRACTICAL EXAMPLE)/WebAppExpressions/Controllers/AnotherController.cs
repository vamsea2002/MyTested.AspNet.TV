using Microsoft.AspNetCore.Mvc;
using WebAppExpressions.Infrastructure;

namespace WebAppExpressions.Controllers
{
    public class AnotherController : Controller
    {
        public IActionResult About()
        {
            var id = 5;

            return this.RedirectTo<HomeController>(c => c.Index(id, "MyTestedASP.NET"));

            // return this.RedirectToAction("Index", "Home", new { id = 5, query = "MyTestedASP.NET" });
        }
    }
}
