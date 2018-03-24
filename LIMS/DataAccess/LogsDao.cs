using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using LIMS.Controllers;
using LIMS.Models;

namespace LIMS.DataAccess
{
    public class LogsDao
    {
        private const int EntriesPerPage = 50;

        public static async Task<List<LogEntry>> List(IRequestContext context, int? pageNumber)
        {
            var query =
                from le in context.DbContext.LogEntries
                orderby le.Date descending
                select le;

            var queryResults = await query
                .Include(le => le.User)
                .Skip((pageNumber ?? 0) * EntriesPerPage)
                .Take(EntriesPerPage)
                .ToListAsync();

            return queryResults;
        }

        public static async Task Create(IRequestContext context, string message)
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