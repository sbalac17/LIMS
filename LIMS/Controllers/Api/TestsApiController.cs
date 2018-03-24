using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using LIMS.DataAccess;
using LIMS.Models;

namespace LIMS.Controllers.Api
{
    [Route("api/tests/")]
    public class TestsApiController : ApiControllerBase
    {
        [Route("api/tests/")]
        [HttpGet]
        [Authorize]
        public async Task<IEnumerable<TestsSearchViewModel.Result>> List(string query = null)
        {
            return await TestsDao.Find(this, query);
        }

        [Route("api/tests/")]
        [HttpPost]
        [Authorize(Roles = Roles.Privileged)]
        [ValidateModel]
        public async Task<Test> Create(TestsCreateViewModel model)
        {
            return await TestsDao.Create(this, model);
        }

        [Route("api/tests/{testCode}")]
        [HttpGet]
        public async Task<IHttpActionResult> Read(string testCode)
        {
            var result = await TestsDao.Read(this, testCode);
            if (result == null)
                return NotFound();

            return Json(result);
        }

        [Route("api/tests/{testCode}")]
        [HttpPut]
        [Authorize(Roles = Roles.Privileged)]
        [ValidateModel]
        public async Task<IHttpActionResult> Update(string testCode, TestsEditViewModel model)
        {
            var test = await TestsDao.Read(this, testCode);
            if (test == null)
                return NotFound();

            var result = await TestsDao.Update(this, test, model);
            return Json(result);
        }

        [Route("api/tests/{testCode}")]
        [HttpDelete]
        [Authorize(Roles = Roles.Administrator)]
        [ValidateModel]
        public async Task<IHttpActionResult> Delete(string testCode)
        {
            var result = await TestsDao.Delete(this, testCode);
            if (result == null)
                return NotFound();

            return Json(result);
        }
    }
}
