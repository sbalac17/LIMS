using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LIMS.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace LIMS
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class LabMemberAttribute : ActionFilterAttribute
    {
        public bool LabManager { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.ActionParameters.TryGetValue("lab", out var labParam))
                throw new InvalidOperationException("Action has no `lab` parameter.");

            if (labParam == null)
            {
                filterContext.Result = new HttpUnauthorizedResult(); // lab doesn't exist
                return;
            }

            if (!(labParam is Lab lab))
                throw new InvalidOperationException("'lab' parameter is not an instance of Lab.");

            var owinContext = filterContext.HttpContext.GetOwinContext();
            var userId = filterContext.HttpContext.User?.Identity?.GetUserId();
            if (userId == null)
            {
                filterContext.Result = new HttpUnauthorizedResult(); // not logged in?
                return;
            }
            
            var userManager = owinContext.GetUserManager<ApplicationUserManager>();
            var user = userManager.FindById(userId);
            if (user == null)
            {
                filterContext.Result = new HttpUnauthorizedResult(); // something messed up
                return;
            }

            var membership = lab.Members.FirstOrDefault(lm => (!LabManager || lm.IsLabManager) && lm.UserId == user.Id);
            if (membership == null)
            {
                filterContext.Result = new HttpUnauthorizedResult(); // not a lab member
                return;
            }
            
            // successfully accessed, update the date in membership
            membership.LastOpened = DateTimeOffset.Now;

            var dbContext = owinContext.Get<ApplicationDbContext>();
            dbContext.SaveChanges();
        }
    }
}
