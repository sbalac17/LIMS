using System.Net.Http;
using System.Web;
using Microsoft.Owin;

namespace LIMS
{
    public static class OwinExtensions
    {
        public static IOwinContext GetOwinContext(this HttpRequestMessage request)
        {
            var context = request.Properties["MS_HttpContext"] as HttpContextWrapper;
            if (context != null)
            {
                return HttpContextBaseExtensions.GetOwinContext(context.Request);
            }

            return null;
        }
    }
}
