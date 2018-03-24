using System.Threading.Tasks;
using System.Web.Mvc;
using LIMS.DataAccess;
using LIMS.Models;

namespace LIMS.Controllers
{
    [Authorize(Roles = Roles.Privileged)]
    public class LogsController : ControllerBase
    {
        public async Task<ActionResult> Index(int? pageNumber)
        {
            var results = await LogsDao.List(this, pageNumber);

            var model = new LogsViewModel
            {
                PageNumber = 0,
                Entries = results
            };

            return View(model);
        }
    }
}
