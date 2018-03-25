using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using LIMS.DataAccess;
using LIMS.Models;
using LIMS.Models.Api;

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

        [Route("api/labs/{labId:int}/report")]
        [HttpGet]
        [Authorize]
        [LabIdMember]
        public async Task<IHttpActionResult> Report(long labId)
        {
            var result = await LabsDao.GenerateReport(this, labId);
            if (result == null)
                return NotFound();

            return Json(result);
        }
        
        /*******************************************/
        
        [Route("api/labs/{labId:int}/members")]
        [HttpGet]
        [Authorize]
        [LabIdMember]
        public async Task<IHttpActionResult> Members(long labId)
        {
            return Json(await LabsDao.ListMembers(this, labId));
        }

        [Route("api/labs/{labId:int}/addableMembers")]
        [HttpGet]
        [Authorize]
        [LabIdMember(LabManager = true)]
        public async Task<IHttpActionResult> AddableMembers(long labId, string query = null)
        {
            return Json(await LabsDao.ListAddableMembers(this, labId, query));
        }

        [Route("api/labs/{labId:int}/addMember/{userId}")]
        [HttpPost]
        [Authorize]
        [LabIdMember(LabManager = true)]
        public async Task<IHttpActionResult> AddMember(long labId, string userId)
        {
            return Json(await LabsDao.AddMember(this, labId, userId));
        }

        [Route("api/labs/{labId:int}/removeMember/{userId}")]
        [HttpPost]
        [Authorize]
        [LabIdMember(LabManager = true)]
        public async Task<IHttpActionResult> RemoveMember(long labId, string userId)
        {
            return Json(await LabsDao.RemoveMember(this, labId, userId));
        }
        
        /*******************************************/
        
        [Route("api/labs/{labId:int}/reagents")]
        [HttpGet]
        [Authorize]
        [LabIdMember]
        public async Task<IHttpActionResult> Reagents(long labId)
        {
            return Json(await LabsDao.ListReagents(this, labId));
        }

        [Route("api/labs/{labId:int}/addReagent/{reagentId:int}")]
        [HttpPost]
        [Authorize]
        [LabIdMember(LabManager = true)]
        public async Task<IHttpActionResult> AddReagent(long labId, long reagentId, int quantity = 1)
        {
            var result = await LabsDao.AddReagent(this, labId, reagentId, quantity);
            if (result == null)
                return NotFound();

            return Json(result);
        }

        [Route("api/labs/{labId:int}/removeReagent/{reagentId:int}")]
        [HttpPost]
        [Authorize]
        [LabIdMember(LabManager = true)]
        public async Task<IHttpActionResult> RemoveReagent(long labId, long reagentId, int returnQuantity = 0)
        {
            var result = await LabsDao.RemoveReagent(this, labId, reagentId, returnQuantity);
            if (result == null)
                return NotFound();

            return Json(result);
        }
        
        /*******************************************/
        
        [Route("api/labs/{labId:int}/samples")]
        [HttpGet]
        [Authorize]
        [LabIdMember]
        public async Task<IHttpActionResult> Samples(long labId, string query = null)
        {
            return Json(await LabsDao.ListSamples(this, labId, query));
        }
        
        [Route("api/labs/{labId:int}/samples/{sampleId:int}")]
        [HttpGet]
        [Authorize]
        [LabIdMember]
        public async Task<IHttpActionResult> SampleDetails(long labId, long sampleId)
        {
            var result = await LabsDao.GetSampleDetails(this, labId, sampleId);
            if (result == null)
                return NotFound();

            return Json(result);
        }
        
        [Route("api/labs/{labId:int}/samples/{sampleId:int}/comment")]
        [HttpPost]
        [Authorize]
        [LabIdMember]
        [ValidateModel]
        public async Task<IHttpActionResult> Comment(long labId, long sampleId, LabsApiCommentModel model)
        {
            var result = await LabsDao.PostComment(this, labId, sampleId, model);
            if (result == null)
                return NotFound();

            return Json(result);
        }
        
        [Route("api/labs/{labId:int}/samples/{sampleId:int}")]
        [HttpPut]
        [Authorize]
        [LabIdMember(LabManager = true)]
        [ValidateModel]
        public async Task<IHttpActionResult> UpdateSample(long labId, long sampleId, LabsApiEditSampleModel model)
        {
            var result = await LabsDao.UpdateSample(this, labId, sampleId, model);
            if (result == null)
                return NotFound();

            return Json(result);
        }
        
        [Route("api/labs/{labId:int}/addSample/{sampleId:int}")]
        [HttpPost]
        [Authorize]
        [LabIdMember(LabManager = true)]
        public async Task<IHttpActionResult> AddSample(long labId, long sampleId)
        {
            var result = await LabsDao.AddSample(this, labId, sampleId);
            if (result == null)
                return NotFound();

            return Json(result);
        }

        [Route("api/labs/{labId:int}/removeSample/{sampleId:int}")]
        [HttpPost]
        [Authorize]
        [LabIdMember(LabManager = true)]
        public async Task<IHttpActionResult> RemoveSample(long labId, long sampleId)
        {
            var result = await LabsDao.RemoveSample(this, labId, sampleId);
            if (result == null)
                return NotFound();

            return Json(result);
        }
    }
}
