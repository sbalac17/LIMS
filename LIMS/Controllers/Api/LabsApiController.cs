using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using LIMS.DataAccess;
using LIMS.Models;

namespace LIMS.Controllers.Api
{
    [Route("api/labs/")]
    public class LabsApiController : ApiControllerBase
    {
        [Route("api/labs/")]
        [HttpGet]
        [Authorize]
        public async Task<IEnumerable<LabsSearchResult>> List(string query = null)
        {
            return await LabsDao.Find(this, query);
        }

        [Route("api/labs/")]
        [HttpPost]
        [Authorize(Roles = Roles.Privileged)]
        [ValidateModel]
        public async Task<Lab> Create(LabsCreateViewModel model)
        {
            return await LabsDao.Create(this, model);
        }

        [Route("api/labs/{labId:int}")]
        [HttpGet]
        [Authorize]
        [LabIdMember]
        public async Task<IHttpActionResult> Read(long labId)
        {
            var result = await LabsDao.Read(this, labId);
            if (result == null)
                return NotFound();

            return Json(result);
        }

        [Route("api/labs/{labId:int}")]
        [HttpPut]
        [Authorize]
        [LabIdMember(LabManager = true)]
        [ValidateModel]
        public async Task<IHttpActionResult> Update(long labId, LabsEditViewModel model)
        {
            var lab = await LabsDao.Read(this, labId);
            if (lab == null)
                return NotFound();

            var result = await LabsDao.Update(this, lab, model);
            return Json(result);
        }

        [Route("api/labs/{labId:int}")]
        [HttpDelete]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<IHttpActionResult> Delete(long labId)
        {
            var result = await LabsDao.Delete(this, labId);
            if (result == null)
                return NotFound();

            return Json(result);
        }
    }
}
