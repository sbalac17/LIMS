using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using LIMS.Controllers;
using LIMS.Models;

namespace LIMS.DataAccess
{
    public static class HomeDao
    {
        public static async Task<List<Lab>> RecentLabs(IRequestContext context)
        {
            var recentLabs =
                from lm in context.DbContext.LabMembers
                where lm.UserId == context.UserId
                orderby lm.LastOpened descending
                select lm.Lab;

            return await recentLabs.Take(2).ToListAsync();
        }
    }
}