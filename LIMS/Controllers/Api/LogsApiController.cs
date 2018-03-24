using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using LIMS.DataAccess;
using LIMS.Models;

namespace LIMS.Controllers.Api
{
    [Route("api/logs/")]
    public class LogsApiController : ApiControllerBase
    {
        [Route("api/logs/")]
        [HttpGet]
        [Authorize]
        public async Task<IEnumerable<LogEntry>> List(int pageNumber = 0)
        {
            return await LogsDao.List(this, pageNumber);
        }
    }
}
