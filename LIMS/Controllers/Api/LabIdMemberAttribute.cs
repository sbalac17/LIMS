using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using LIMS.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace LIMS.Controllers.Api
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class LabIdMemberAttribute : ActionFilterAttribute
    {
        public bool LabManager { get; set; }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            if (!actionContext.ActionArguments.TryGetValue("labId", out var labParam))
                throw new InvalidOperationException("Action has no `labId` parameter.");

            if (labParam == null)
            {
                actionContext.Response = Unauthorized(actionContext); // lab doesn't exist
                return;
            }

            if (!(labParam is long labId))
                throw new InvalidOperationException("'labId' parameter is not an Int64.");

            var owinContext = actionContext.Request.GetOwinContext();
            var userId = actionContext.RequestContext.Principal?.Identity?.GetUserId();
            if (userId == null)
            {
                actionContext.Response = Unauthorized(actionContext); // not logged in?
                return;
            }
            
            var userManager = owinContext.GetUserManager<ApplicationUserManager>();
            var user = userManager.FindById(userId);
            if (user == null)
            {
                actionContext.Response = Unauthorized(actionContext); // something messed up
                return;
            }
            
            var dbContext = owinContext.Get<ApplicationDbContext>();
            var membership = dbContext.LabMembers.FirstOrDefault(lm => lm.LabId == labId && (!LabManager || lm.IsLabManager) && lm.UserId == user.Id);
            if (membership == null)
            {
                actionContext.Response = Unauthorized(actionContext); // not a lab member
                return;
            }
            
            // successfully accessed, update the date in membership
            membership.LastOpened = DateTimeOffset.Now;

            dbContext.SaveChanges();
        }

        private static HttpResponseMessage Unauthorized(HttpActionContext actionContext) =>
            actionContext.Request.CreateErrorResponse(HttpStatusCode.Forbidden, "Unauthorized");
    }
}