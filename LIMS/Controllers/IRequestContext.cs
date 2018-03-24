using System.Threading.Tasks;
using LIMS.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace LIMS.Controllers
{
    public interface IRequestContext
    {
        ApplicationUserManager UserManager { get; }

        RoleManager<IdentityRole> RoleManager { get; }

        ApplicationDbContext DbContext { get; }

        string UserId { get; }

        string UserName { get; }

        Task LogAsync(string message);
    }
}
