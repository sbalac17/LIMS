using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using LIMS.DataAccess;
using LIMS.Models;

namespace LIMS.Controllers.Api
{
    [Route("api/samples/")]
    public class SamplesApiController : ApiControllerBase
    {
        [Route("api/samples/")]
        [HttpGet]
        [Authorize]
        public async Task<IEnumerable<SamplesSearchViewModel.Result>> List(string query = null)
        {
            return await SamplesDao.Find(this, query);
        }

        [Route("api/samples/")]
        [HttpPost]
        [Authorize(Roles = Roles.Privileged)]
        [ValidateModel]
        public async Task<Sample> Create(SamplesCreateViewModel model)
        {
            return await SamplesDao.Create(this, model);
        }

        [Route("api/samples/{sampleId:int}")]
        [HttpGet]
        public async Task<IHttpActionResult> Read(int sampleId)
        {
            var result = await SamplesDao.Read(this, sampleId);
            if (result == null)
                return NotFound();

            return Json(result);
        }

        [Route("api/samples/{sampleId:int}")]
        [HttpPut]
        [Authorize(Roles = Roles.Privileged)]
        [ValidateModel]
        public async Task<IHttpActionResult> Update(int sampleId, SamplesEditViewModel model)
        {
            var sample = await SamplesDao.Read(this, sampleId);
            if (sample == null)
                return NotFound();

            var result = await SamplesDao.Update(this, sample, model);
            return Json(result);
        }

        [Route("api/samples/{sampleId:int}")]
        [HttpDelete]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<IHttpActionResult> Delete(int sampleId)
        {
            var result = await SamplesDao.Delete(this, sampleId);
            if (result == null)
                return NotFound();

            return Json(result);
        }
    }
}
