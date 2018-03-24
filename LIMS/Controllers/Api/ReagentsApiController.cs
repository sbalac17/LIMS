using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using LIMS.Models;

namespace LIMS.Controllers.Api
{
    [Route("api/reagents/")]
    [Authorize]
    public class ReagentsApiController : ApiControllerBase
    {
        [HttpGet]
        public async Task<IEnumerable<Reagent>> Get(string query = null)
        {
            IQueryable<Reagent> results;

            if (string.IsNullOrWhiteSpace(query))
            {
                results = from r in DbContext.Reagents
                    orderby r.Name
                    select r;
            }
            else
            {
                results = from r in DbContext.Reagents
                    where r.Name.Contains(query)
                    orderby r.Name
                    select r;
            }

            return await results.ToListAsync();
        }
    }
}
