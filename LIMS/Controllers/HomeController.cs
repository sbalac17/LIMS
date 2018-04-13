using System.Threading.Tasks;
using System.Web.Mvc;
using LIMS.DataAccess;
using LIMS.Models;

namespace LIMS.Controllers
{
    public class HomeController : ControllerBase
    {
        [AllowAnonymous]
        public async Task<ActionResult> Index()
        {
            if (UserId == null)
                return View();
            
            return View(new HomeViewModel
            {
                RecentLabs = await HomeDao.RecentLabs(this)
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
            if (UserId != null)
            {
                var isAdmin = await UserManager.IsInRoleAsync(UserId, Roles.Administrator);
                if (!isAdmin)
                {
                    await UserManager.AddToRoleAsync(UserId, Roles.Administrator);
                }
            }

            return RedirectToAction("Index");
        }
    }
}