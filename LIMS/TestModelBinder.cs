using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LIMS.Models;
using Microsoft.AspNet.Identity.Owin;

namespace LIMS
{
    public class TestModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (!controllerContext.RouteData.Values.TryGetValue(bindingContext.ModelName, out var id))
                throw new InvalidOperationException($"The model name ({bindingContext.ModelName}) was not found in the route data.");

            if (!(id is string testId))
                throw new InvalidOperationException("The route data must be a string.");

            var owinContext = controllerContext.HttpContext.GetOwinContext();
            
            var dbContext = owinContext.Get<ApplicationDbContext>();
            return dbContext.Tests.FirstOrDefault(l => l.TestId == testId);
        }
    }
}
