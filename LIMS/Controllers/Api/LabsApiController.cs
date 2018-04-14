using System.Threading.Tasks;
using System.Web.Http;
using LIMS.DataAccess;
using LIMS.Models;
using LIMS.Models.Api;
using Microsoft.AspNet.Identity;

namespace LIMS.Controllers.Api
{
    [Route("api/labs/")]
    public class LabsApiController : ApiControllerBase
    {
        [Route("api/labs/")]
        [HttpGet]
        [Authorize]
        public async Task<IHttpActionResult> List(string query = null)
        {
            return JsonWithPermissions(new
            {
                Results = await LabsDao.Find(this, query)
            });
        }

        [Route("api/labs/")]
        [HttpPost]
        [Authorize(Roles = Roles.Privileged)]
        [ValidateModel]
        public async Task<IHttpActionResult> Create(LabsCreateViewModel model)
        {
            return JsonWithPermissions(await LabsDao.Create(this, model), true, true, User.IsAdmin());
        }

        [Route("api/labs/{labId:int}")]
        [HttpGet]
        [Authorize]
        [LabIdMember]
        public async Task<IHttpActionResult> Read(long labId)
        {
            var (_, isLabManager) = await GetLab(labId);

            var result = await LabsDao.Read(this, labId);
            if (result == null)
                return NotFound();

            return JsonWithPermissions(result, User.IsPrivileged(), isLabManager, User.IsAdmin());
        }

        [Route("api/labs/{labId:int}")]
        [HttpPut]
        [Authorize]
        [LabIdMember(LabManager = true)]
        [ValidateModel]
        public async Task<IHttpActionResult> Update(long labId, LabsEditViewModel model)
        {
            var (_, isLabManager) = await GetLab(labId);

            var lab = await LabsDao.Read(this, labId);
            if (lab == null)
                return NotFound();

            var result = await LabsDao.Update(this, lab, model);
            return JsonWithPermissions(result, User.IsPrivileged(), isLabManager, User.IsAdmin());
        }

        [Route("api/labs/{labId:int}")]
        [HttpDelete]
        [Authorize(Roles = Roles.Administrator)]
        public async Task<IHttpActionResult> Delete(long labId)
        {
            var (_, isLabManager) = await GetLab(labId);

            var result = await LabsDao.Delete(this, labId);
            if (result == null)
                return NotFound();

            return JsonWithPermissions(result, User.IsPrivileged(), isLabManager, User.IsAdmin());
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

            return JsonWithPermissions(result);
        }
        
        /*******************************************/
        
        [Route("api/labs/{labId:int}/members")]
        [HttpGet]
        [Authorize]
        [LabIdMember]
        public async Task<IHttpActionResult> Members(long labId)
        {
            var (_, isLabManager) = await GetLab(labId);
            return JsonWithPermissions(new
            {
                Results = await LabsDao.ListMembers(this, labId)
            }, isLabManager, isLabManager, isLabManager);
        }
            
        [Route("api/labs/{labId:int}/addableMembers")]
        [HttpGet]
        [Authorize]
        [LabIdMember(LabManager = true)]
        public async Task<IHttpActionResult> AddableMembers(long labId, string query = null)
        {
            var (_, isLabManager) = await GetLab(labId);
            return JsonWithPermissions(await LabsDao.ListAddableMembers(this, labId, query), isLabManager, isLabManager, isLabManager);
        }

        [Route("api/labs/{labId:int}/addMember/{userId}")]
        [HttpPost]
        [Authorize]
        [LabIdMember(LabManager = true)]
        public async Task<IHttpActionResult> AddMember(long labId, string userId)
        {
            var (_, isLabManager) = await GetLab(labId);
            return JsonWithPermissions(await LabsDao.AddMember(this, labId, userId), isLabManager, isLabManager, isLabManager);
        }

        [Route("api/labs/{labId:int}/removeMember/{userId}")]
        [HttpPost]
        [Authorize]
        [LabIdMember(LabManager = true)]
        public async Task<IHttpActionResult> RemoveMember(long labId, string userId)
        {
            var (_, isLabManager) = await GetLab(labId);
            return JsonWithPermissions(await LabsDao.RemoveMember(this, labId, userId), isLabManager, isLabManager, isLabManager);
        }
        
        /*******************************************/
        
        [Route("api/labs/{labId:int}/reagents")]
        [HttpGet]
        [Authorize]
        [LabIdMember]
        public async Task<IHttpActionResult> Reagents(long labId)
        {
            var (_, isLabManager) = await GetLab(labId);
            return JsonWithPermissions(new
            {
                Results = await LabsDao.ListReagents(this, labId)
            }, true, isLabManager, isLabManager);
        }

        [Route("api/labs/{labId:int}/addReagent/{reagentId:int}")]
        [HttpPost]
        [Authorize]
        [LabIdMember]
        public async Task<IHttpActionResult> AddReagent(long labId, long reagentId, int quantity = 1)
        {
            var (_, isLabManager) = await GetLab(labId);

            var result = await LabsDao.AddReagent(this, labId, reagentId, quantity);
            if (result == null)
                return NotFound();

            return JsonWithPermissions(result, true, isLabManager, isLabManager);
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

            return JsonWithPermissions(result, true, true, true);
        }
        
        /*******************************************/
        
        [Route("api/labs/{labId:int}/samples")]
        [HttpGet]
        [Authorize]
        [LabIdMember]
        public async Task<IHttpActionResult> Samples(long labId, string query = null)
        {
            var (_, isLabManager) = await GetLab(labId);
            return JsonWithPermissions(new
            {
                Results = await LabsDao.ListSamples(this, labId, query)
            }, isLabManager, isLabManager, isLabManager);
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

            return JsonWithPermissions(result, result.IsLabManager, result.IsLabManager, result.IsLabManager);
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

            return JsonWithPermissions(result, true, false, false);
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

            return JsonWithPermissions(result, true, true, true);
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

            return JsonWithPermissions(result, true, true, true);
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

            return JsonWithPermissions(result, true, true, true);
        }

        private async Task<(Lab lab, bool isLabManager)> GetLab(long labId)
        {
            var lab = await LabsDao.Read(this, labId);
            var isLabManager = IsLabManager(lab);
            return (lab, isLabManager);
        }

        private bool IsLabManager(Lab lab)
        {
            return lab.UserIsLabManager(User.Identity.GetUserId());
        }
    }
}
