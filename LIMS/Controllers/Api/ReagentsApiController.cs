using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using LIMS.DataAccess;
using LIMS.Models;

namespace LIMS.Controllers.Api
{
    [Route("api/reagents/")]
    public class ReagentsApiController : ApiControllerBase
    {
        [Route("api/reagents/")]
        [HttpGet]
        [Authorize]
        public async Task<IEnumerable<Reagent>> List(string query = null)
        {
            return await ReagentsDao.Find(this, query);
        }

        [Route("api/reagents/")]
        [HttpPost]
        [Authorize(Roles = Roles.Privileged)]
        [ValidateModel]
        public async Task<Reagent> Create(ReagentsCreateViewModel model)
        {
            return await ReagentsDao.Create(this, model);
        }

        [Route("api/reagents/{reagentId:int}")]
        [HttpGet]
        public async Task<IHttpActionResult> Read(int reagentId)
        {
            var result = await ReagentsDao.Read(this, reagentId);
            if (result == null)
                return NotFound();

            return Json(result);
        }

        [Route("api/reagents/{reagentId:int}")]
        [HttpPut]
        [Authorize(Roles = Roles.Privileged)]
        [ValidateModel]
        public async Task<IHttpActionResult> Update(int reagentId, ReagentsEditViewModel model)
        {
            var reagent = await ReagentsDao.Read(this, reagentId);
            if (reagent == null)
                return NotFound();

            var result = await ReagentsDao.Update(this, reagent, model);
            return Json(result);
        }

        [Route("api/reagents/{reagentId:int}")]
        [HttpDelete]
        [Authorize(Roles = Roles.Administrator)]
        [ValidateModel]
        public async Task<IHttpActionResult> Delete(int reagentId)
        {
            var result = await ReagentsDao.Delete(this, reagentId);
            if (result == null)
                return NotFound();

            return Json(result);
        }
    }
}
