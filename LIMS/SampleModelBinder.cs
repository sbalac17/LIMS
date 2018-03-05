using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LIMS.Models;
using Microsoft.AspNet.Identity.Owin;

namespace LIMS
{
    public class SampleModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (!controllerContext.RouteData.Values.TryGetValue(bindingContext.ModelName, out var id))
                throw new InvalidOperationException($"The model name ({bindingContext.ModelName}) was not found in the route data.");

            if (!(id is string idStr))
                throw new InvalidOperationException("The route data must be a string.");

            if (!long.TryParse(idStr, out var sampleId))
                throw new InvalidOperationException("The route data must be parsable to an Int64.");

            var owinContext = controllerContext.HttpContext.GetOwinContext();
            
            var dbContext = owinContext.Get<ApplicationDbContext>();
            return dbContext.Samples.FirstOrDefault(l => l.SampleId == sampleId);
        }
    }
}