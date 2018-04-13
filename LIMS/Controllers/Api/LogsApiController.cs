using System.Threading.Tasks;
using System.Web.Http;
using LIMS.DataAccess;

namespace LIMS.Controllers.Api
{
    [Route("api/logs/")]
    public class LogsApiController : ApiControllerBase
    {
        [Route("api/logs/")]
        [HttpGet]
        [Authorize]
        public async Task<IHttpActionResult> List(int pageNumber = 0)
        {
            return Json(new
            {
                Results = await LogsDao.List(this, pageNumber)
            });
        }
    }
}
