namespace Sigma.Web.Frontend.Core.Filters
{
    using System;
    using System.Web.Http.Filters;
    using System.Diagnostics.Contracts;
    using System.Net.Http.Headers;

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class NoResponseCacheAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext context)
        {
            Contract.Assert(context != null);
            if (context.Response != null)
            {
                Contract.Assert(context.Response.Headers != null);
                context.Response.Headers.CacheControl = new CacheControlHeaderValue()
                {
                    NoCache = true,
                    NoStore = true,
                    MaxAge = TimeSpan.Zero,
                    MustRevalidate = true
                };
                context.Response.Headers.Pragma.Add(new NameValueHeaderValue("no-cache"));
            }

            base.OnActionExecuted(context);
        }
    }
}