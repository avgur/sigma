[assembly: Microsoft.Owin.OwinStartup(typeof(Sigma.Web.Frontend.FrontendStartup))]

namespace Sigma.Web.Frontend
{
    using Microsoft.Owin;
    using Microsoft.Owin.FileSystems;
    using Microsoft.Owin.StaticFiles;
    using Owin;

    public class FrontendStartup
    {
        public virtual void Configuration(IAppBuilder app)
        {
            // app.UseCommonLogging();

            // https://www.majormojo.co.uk/blog/deploying-application-and-owin-authorization-server-on-separate-machines.html
            // app.SetDataProtectionProvider(new MachineKeyDataProtectionProvider());

            WebApiBootstrap.Configuration(app);
            this.ConfigureWeb(app);
        }

        #region Web

        private void ConfigureWeb(IAppBuilder app)
        {
            app.UseStaticFiles(new StaticFileOptions
            {
                RequestPath = new PathString(""),
                FileSystem = new PhysicalFileSystem(@".\public"),
                OnPrepareResponse = ctx =>
                {
                    ctx.OwinContext.Response.Headers.Append("Cache-Control", "public,max-age=1209600");
                }
            });

            app.UseStaticFiles(new StaticFileOptions
            {
                RequestPath = new PathString("/app/1.0"),
                FileSystem = new PhysicalFileSystem(@".\public\app"),
                OnPrepareResponse = ctx =>
                {
                    ctx.OwinContext.Response.Headers.Append("Cache-Control", "public,max-age=1209600");
                }
            });
        }

        #endregion
    }
}
