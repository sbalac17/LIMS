using System.Threading.Tasks;
using LIMS.Models;

namespace LIMS.Controllers
{
    public interface IRequestContext
    {
        ApplicationDbContext DbContext { get; }

        string UserId { get; }

        string UserName { get; }

        Task LogAsync(string message);
    }
}
