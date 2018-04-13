using System.Threading.Tasks;
using System.Web.Http;
using LIMS.DataAccess;

namespace LIMS.Controllers.Api
{
    [Route("api/home/")]
    public class HomeApiController : ApiControllerBase
    {
        [Route("api/home/")]
        [HttpGet]
        [Authorize]
        public async Task<IHttpActionResult> RecentLabs()
        {
            return Json(new
            {
                RecentLabs = await HomeDao.RecentLabs(this)
            });
        }
    }
}
