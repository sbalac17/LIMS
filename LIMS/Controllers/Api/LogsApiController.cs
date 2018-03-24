using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using LIMS.DataAccess;
using LIMS.Models;

namespace LIMS.Controllers.Api
{
    [Route("api/logs/")]
    public class LogsApiController : ApiControllerBase
    {
        // copy of LogEntry with reduced information
        public class Entry
        {
            public string UserId { get; }

            public string Message { get; }
            
            [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy h:mm:ss tt}", ApplyFormatInEditMode = true)]
            public DateTimeOffset Date { get; set; }

            public Entry(LogEntry entry)
            {
                UserId = entry.UserId;
                Message = entry.Message;
                Date = entry.Date;
            }
        }

        [Route("api/logs/")]
        [HttpGet]
        [Authorize]
        public async Task<IEnumerable<Entry>> List(int pageNumber = 0)
        {
            var results = await LogsDao.List(this, pageNumber);
            return results.Select(e => new Entry(e));
        }
    }
}
