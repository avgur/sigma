
namespace Sigma.Web.Http.Extensions
{
    using Microsoft.Owin;
    using Sigma.Web.Frontend.Core.DataHttp.Middleware;
    using System.Net.Http;
    using System.Web;

    public static class HttpRequestMessageExtensions
    {
        private static string CallerDetailsKey = typeof(CallerDetails).Name;

        public static string GetTrackingId(this HttpRequestMessage req)
        {
            return req.GetOwinContext().GetTrackingId();
        }

        public static string GetTrackingId(this IOwinContext context)
        {
            return context.GetCallerDetails()?.TrackingId;
        }

        public static void SetCallerDetails(this IOwinContext context, CallerDetails callDetails)
        {
            context.Request.Set(CallerDetailsKey, callDetails);
        }

        public static CallerDetails GetCallerDetails(this IOwinContext context)
        {
            return context.Request.Get<CallerDetails>(CallerDetailsKey);
        }
    }
}