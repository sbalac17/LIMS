using System.Collections.Generic;

namespace LIMS.Models
{
    public class LogsViewModel
    {
        public int PageNumber { get; set; }

        public List<LogEntry> Entries { get; set; }
    }
}
