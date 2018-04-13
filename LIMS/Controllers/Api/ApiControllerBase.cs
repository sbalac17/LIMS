using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.Mvc;
using LIMS.DataAccess;
using LIMS.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LIMS.Controllers.Api
{
    public class ApiControllerBase : ApiController, IRequestContext
    {
        private ApplicationUserManager _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private ApplicationDbContext _dbContext;
        private string _userId;
        private string _username;

        public ApplicationUserManager UserManager =>
            _userManager ?? (_userManager = Request.GetOwinContext().GetUserManager<ApplicationUserManager>());

        public RoleManager<IdentityRole> RoleManager =>
            _roleManager ?? (_roleManager = Request.GetOwinContext().Get<ApplicationRoleManager>());

        public ApplicationDbContext DbContext =>
            _dbContext ?? (_dbContext = Request.GetOwinContext().Get<ApplicationDbContext>());

        public string UserId =>
            _userId ?? (_userId = RequestContext?.Principal?.Identity?.GetUserId());

        public string UserName =>
            _username ?? (_username = RequestContext?.Principal?.Identity?.GetUserName());

        public Task LogAsync(string message)
        {
            return LogsDao.Create(this, message);
        }

        protected IHttpActionResult JsonWithPermissions<T>(T value) =>
            JsonWithPermissions(value, User.IsPrivileged(), User.IsPrivileged(), User.IsAdmin());

        protected IHttpActionResult JsonWithPermissions<T>(T value, bool canCreate, bool canUpdate, bool canDelete)
        {
            var result = JObject.FromObject(value);
            var permissions = new JObject
            {
                { "CanCreate", canCreate },
                { "CanUpdate", canUpdate },
                { "CanDelete", canDelete },
            };
            result.Add("$permissions", permissions);

            return Json(result);
        }
    }
}
