using System.Threading.Tasks;
using System.Web.Http;
using LIMS.DataAccess;
using LIMS.Models;
using Microsoft.AspNet.Identity;

namespace LIMS.Controllers.Api
{
    public class ApiControllerBase : ApiController, IRequestContext
    {
        private ApplicationDbContext _dbContext;
        private string _userId;
        private string _username;

        public ApplicationDbContext DbContext =>
            _dbContext ?? (_dbContext = ApplicationDbContext.Create());

        public string UserId =>
            _userId ?? (_userId = RequestContext?.Principal?.Identity?.GetUserId());

        public string UserName =>
            _username ?? (_username = RequestContext?.Principal?.Identity?.GetUserName());

        public Task LogAsync(string message)
        {
            return LogsDao.Add(this, message);
        }
    }
}
