using System.Threading.Tasks;
using System.Web.Http;
using LIMS.DataAccess;
using LIMS.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

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
    }
}
