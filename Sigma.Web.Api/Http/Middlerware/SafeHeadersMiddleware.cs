namespace Sigma.Web.Http.Middlerware
{
    using Microsoft.Owin;
    using System.Threading.Tasks;

    public class SafeHeadersMiddleware : OwinMiddleware
    {
        public SafeHeadersMiddleware(OwinMiddleware next)
            : base(next)
        {
        }

        public async override Task Invoke(IOwinContext context)
        {
            context.Response.Headers.Remove("Server");
            await Next.Invoke(context);
        }
    }
}