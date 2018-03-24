using System;
using System.Threading.Tasks;
using LIMS.Controllers;
using LIMS.Models;

namespace LIMS.DataAccess
{
    public class LogsDao
    {
        public static async Task Add(IRequestContext context, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentNullException(nameof(message));

            var entry = new LogEntry
            {
                UserId = context.UserId,
                Message = message,
                Date = DateTimeOffset.Now
            };

            context.DbContext.LogEntries.Add(entry);
            await context.DbContext.SaveChangesAsync();
        }
    }
}