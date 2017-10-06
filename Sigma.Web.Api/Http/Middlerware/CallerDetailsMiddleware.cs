
namespace Sigma.Web.Http.Middlerware
{
    using Microsoft.Owin;
    using Sigma.Web.Frontend.Core.DataHttp.Middleware;
    using Sigma.Web.Http.Extensions;
    using System;
    using System.Threading.Tasks;

    public class CallerDetailsMiddleware : OwinMiddleware
    {
        public CallerDetailsMiddleware(OwinMiddleware next)
            : base(next)
        {
        }

        public async override Task Invoke(IOwinContext context)
        {
            var callDetails = new CallerDetails
            {
                TrackingId = Guid.NewGuid().ToString("N"),
                OperationDate = DateTime.UtcNow,
                RemoteIpAddress = context.Request.RemoteIpAddress,
                LocalIpAddress = context.Request.LocalIpAddress,
                UserAgent = context.Request.Headers.Get("User-Agent"),
                Method = context.Request.Method,
                Uri = context.Request.Uri.AbsoluteUri
            };

            context.SetCallerDetails(callDetails);

            await Next.Invoke(context);
        }
    }
}