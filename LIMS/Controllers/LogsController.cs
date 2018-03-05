using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using LIMS.Models;

namespace LIMS.Controllers
{
    [Authorize(Roles = Roles.Privileged)]
    public class LogsController : ControllerBase
    {
        private const int EntriesPerPage = 50;

        public async Task<ActionResult> Index(int? pageNumber)
        {
            // TODO: pagination

            var query =
                from le in DbContext.LogEntries
                orderby le.Date descending
                select le;

            var queryResults = await query
                .Include(le => le.User)
                .Skip((pageNumber ?? 0) * EntriesPerPage)
                .Take(EntriesPerPage)
                .ToListAsync();

            var model = new LogsViewModel
            {
                PageNumber = 0,
                Entries = queryResults
            };

            return View(model);
        }
    }
}
