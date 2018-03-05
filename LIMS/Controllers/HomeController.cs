using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using LIMS.Models;
using Microsoft.AspNet.Identity;

namespace LIMS.Controllers
{
    public class HomeController : ControllerBase
    {
        [AllowAnonymous]
        public ActionResult Index()
        {
            var userId = HttpContext.User?.Identity?.GetUserId();
            if (userId == null)
                return View();

            var recentLabs =
                from lm in DbContext.LabMembers
                where lm.UserId == userId
                orderby lm.LastOpened descending
                select lm.Lab;

            return View(new HomeViewModel
            {
                RecentLabs = recentLabs.Take(2).ToList()
            });
        }
        
        [AllowAnonymous]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        
        [AllowAnonymous]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        
        [Authorize]
        public async Task<ActionResult> Promote()
        {
            var userId = HttpContext.User?.Identity?.GetUserId();
            if (userId != null)
            {
                var isAdmin = await UserManager.IsInRoleAsync(userId, Roles.Administrator);
                if (!isAdmin)
                {
                    await UserManager.AddToRoleAsync(userId, Roles.Administrator);
                }
            }

            return RedirectToAction("Index");
        }
    }
}