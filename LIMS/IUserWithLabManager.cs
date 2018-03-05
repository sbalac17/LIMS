using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LIMS.Models;

namespace LIMS
{
    public interface IUserWithLabManager
    {
        ApplicationUser User { get; }
        bool IsLabManager { get; }
    }
}
