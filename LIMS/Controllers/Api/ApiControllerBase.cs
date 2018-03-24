using System.Web.Http;
using LIMS.Models;

namespace LIMS.Controllers.Api
{
    public class ApiControllerBase : ApiController
    {
        private ApplicationDbContext _dbContext;

        public ApplicationDbContext DbContext =>
            _dbContext ?? (_dbContext = ApplicationDbContext.Create());
    }
}
