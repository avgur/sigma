namespace Infoplus.TenderMarket.Web.Api
{
    using System;
    using System.Configuration;
    using System.Net.Http.Formatting;
    using System.Threading;
    using System.Web.Configuration;
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;

    using Infoplus.TenderMarket.Common.Unity;
    using Infoplus.TenderMarket.Web.Api.Configuration;
    using Infoplus.TenderMarket.Web.Api.Filters;

    using Microsoft.Owin.BuilderProperties;
    using Microsoft.Owin.Security.DataProtection;
    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Practices.Unity;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    using Owin;
    using Owin.Logging.Common;

    using Unity.WebApi;

    // Do not want to use the name Startup, because owin used conventional load 
    // and can instantiate that class directly 
    public abstract partial class Bootstrap
    {
        private readonly Lazy<bool> isDebugMode = new Lazy<bool>(() => ((CompilationSection)ConfigurationManager.GetSection("system.web/compilation")).Debug);

        private bool DebugEnabled
        {
            get
            {
                return this.isDebugMode.Value;
            }
        }

        public virtual void Configuration(IAppBuilder app)
        {
            app.UseCommonLogging();
            app.SetDataProtectionProvider(new MachineKeyDataProtectionProvider());
            
            this.ConfigureShell(app);

            /*app.Use<OwinLoggingMiddleware>();*/
            app.Use<OwinExceptionHandlerMiddleware>();

            this.ConfigureAuth(app);

            this.ConfigureWebApi(app);
        }

        protected virtual void ConfigureShell(IAppBuilder app)
        {
            var properties = new AppProperties(app.Properties);
            var token = properties.OnAppDisposing;

            if (token != CancellationToken.None)
            {
                app.CreatePerOwinContext(
                    () => new AmbientContext(false));
                
                var dataProtectionProvider = app.GetDataProtectionProvider();
                var extension = new WebApiContainerExtension(dataProtectionProvider, this.OnShellExtraInitialization);
                Shell.Start(extension);
                token.Register(Shell.Shutdown);
            }
        }

        protected virtual void OnShellExtraInitialization(IUnityContainer obj)
        {
        }

        protected virtual void ConfigureWebApi(IAppBuilder app)
        {
            var config = new HttpConfiguration
                             {
                                 DependencyResolver =
                                     ServiceLocator.Current.GetInstance<UnityDependencyResolver>(),
                                 IncludeErrorDetailPolicy =
                                     this.DebugEnabled
                                         ? IncludeErrorDetailPolicy.Always
                                         : IncludeErrorDetailPolicy.Never
                             };
            config.MessageHandlers.Add(new LanguageMessageHandler());

            this.ConfigureJsonFormatter(config);

            this.RegisterWebApiRoutes(config);

            this.RegisterWebApiFilters(config.Filters);
            
            app
                // https://www.jayway.com/2016/01/08/improving-error-handling-asp-net-web-api-2-1-owin/
                /*.Use(async (context, next) =>
                {
                    try
                    {
                        await next();
                    }
                    catch (Exception ex)
                    {
                        ex = ex.TransformException(WebApiContainerExtension.DefaultPolicy);
                        var exception = ex as OperationErrorException;
                        if (exception != null)
                        {
                            context.Response.StatusCode = (int)exception.Error.Status;
                            context.Response.ContentType = "application/json";
                            context.Response.Write(JsonConvert.SerializeObject(exception.Error));
                        }
                        else
                        {
                            throw;
                        }
                        
                        #if DEBUG
                        throw; // Optional: Re-throw if debugging, to help troubeshooting
                        #endif
                    }
                })*/
                .UseWebApi(config);
        }

        protected virtual void ConfigureJsonFormatter(HttpConfiguration config)
        {
            config.Formatters.Clear();

            // ReSharper disable once UseObjectOrCollectionInitializer
            var jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            jsonSerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
#if DEBUG
            jsonSerializerSettings.Formatting = Formatting.Indented;
#endif
            config.Formatters.Add(new JsonMediaTypeFormatter { SerializerSettings = jsonSerializerSettings });
        }
    }
}
