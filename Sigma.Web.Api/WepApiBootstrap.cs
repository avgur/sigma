namespace Sigma.Web.Frontend
{
    using Microsoft.Owin;
    using Owin;
    using System.Web.Http;
    using System.Net;
    using System;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using System.Net.Http.Formatting;
    using System.Web.Http.Controllers;
    using System.Web.Http.Dispatcher;
    using System.Collections.Generic;
    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
    using Microsoft.Owin.Security.Cookies;
    using Microsoft.Owin.Security.OAuth;
    using Sigma.Web.IdentityServices;
    using Microsoft.Owin.Security;
    using Sigma.Web.Http.ExceptionHandling;
    using Sigma.Web.Frontend.Core.Filters;
    using Sigma.Web.Http.Dispatcher;
    using Sigma.Web.Http.Middlerware;
    using Sigma.Web.Http.Extensions;
    using Sigma.Web.Http.Exceptions;
    using Sigma.Web.Api;
    using Sigma.Web.Api.Controllers;

    public class WebApiBootstrap
    {
        public static void Configuration(IAppBuilder app)
        {
            new WebApiBootstrap().Configure(app);
        }

        public void Configure(IAppBuilder app)
        {
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            // app.Use<XssFilterMiddleware>();
            // app.Use<DontSniffMimetypeMiddleware>();
            // app.Use<IeNoOpenMiddleware>();
            // app.Use<FrameGuardMiddleware>("deny");
            app.Use<SafeHeadersMiddleware>();
            app.Use<CallerDetailsMiddleware>();

            ConfigureAuth(app);

            var config = BuildHttpConfiguration();
            app.UseWebApi(config);
        }

        #region Auth

        private static void ConfigureAuth(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = Registry.ApplicationCookie,
                // LoginPath = new PathString("/api/value/login"),
                // LogoutPath = new PathString("/api/value/logout"),
                ExpireTimeSpan = TimeSpan.FromMinutes(20),
                SlidingExpiration = true,
                CookieName = "sigma.auth", // CookieName = "{0}.token".Fmt(this.OAuthIssuer)
                CookieHttpOnly = true
            });

            ConfigureJwtProducer(app);
            ConfigureJwtConsumer(app);
        }

        private static void ConfigureJwtProducer(IAppBuilder app)
        {
            var options = new OAuthAuthorizationServerOptions
            {
                // todo For Dev enviroment only (on production should be AllowInsecureHttp = false)
                AllowInsecureHttp = true,//this.DebugEnabled,
                TokenEndpointPath = new PathString("/api/token"),
                AccessTokenExpireTimeSpan = TimeSpan.FromSeconds(Registry.OAuthAccessTokenExpireSeconds),
                Provider = new CustomJwtAuthorizationProvider(Registry.OAuthAllowedAudience),
                RefreshTokenProvider = new CustomJwtRefreshTokenProvider(),
                RefreshTokenFormat = new CustomJwtFormat(Registry.OAuthAllowedAudience, Registry.OAuthIssuer, Registry.OAuthBase64Secret, Registry.OAuthRefreshTokenExpireSeconds, c => c.Type == "sub"),
                AccessTokenFormat = new CustomJwtFormat(Registry.OAuthAllowedAudience, Registry.OAuthIssuer, Registry.OAuthBase64Secret, Registry.OAuthAccessTokenExpireSeconds)
            };
#if DEBUG
            options.AllowInsecureHttp = true;
#endif

            app.UseOAuthAuthorizationServer(options);
        }

        private static void ConfigureJwtConsumer(IAppBuilder app)
        {
            var accessTokenFormat = new CustomJwtFormat(
                Registry.OAuthAllowedAudience,
                Registry.OAuthIssuer,
                Registry.OAuthBase64Secret,
                Registry.OAuthAccessTokenExpireSeconds);

            var options = new OAuthBearerAuthenticationOptions
            {
                AuthenticationType = "Bearer",
                AuthenticationMode = AuthenticationMode.Active,
                AccessTokenFormat = accessTokenFormat
            };

            app.UseOAuthBearerAuthentication(options);
        }

        #endregion

        #region WebApi configuration

        private HttpConfiguration BuildHttpConfiguration()
        {
            var config = new HttpConfiguration();

            config.EnableCors();

            this.ConfigureRoutes(config);
            this.ConfigureFilters(config);
            this.ConfigureFormatters(config);
            this.ConfigureServices(config);

            return config;
        }

        private void ConfigureRoutes(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }

        private void ConfigureFilters(HttpConfiguration config)
        {
            config.MessageHandlers.Add(new CultureMessageHandler("en-US"));

            config.Filters.Add(new ValidateActionParametersAttribute());
            config.Filters.Add(new NoResponseCacheAttribute());
        }

        private void ConfigureFormatters(HttpConfiguration config)
        {
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            config.Formatters.Remove(config.Formatters.JsonFormatter);

            var jsonSerializerSettings = new JsonSerializerSettings();
            // jsonSerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            jsonSerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
#if DEBUG
            jsonSerializerSettings.Formatting = Formatting.Indented;
#endif
            config.Formatters.Add(new JsonMediaTypeFormatter { SerializerSettings = jsonSerializerSettings });
        }

        private void ConfigureServices(HttpConfiguration config)
        {
            var exceptionManager = this.CreateExceptionManager();

            config.Services.Replace(typeof(IAssembliesResolver), new ExplicitAssembliesResolver(typeof(ValueController).Assembly));
            config.Services.Replace(typeof(IHttpControllerSelector), new HttpNotFoundAwareControllerSelector(config));
            config.Services.Replace(typeof(IHttpActionSelector), new HttpNotFoundAwareControllerActionSelector());
            config.Services.Replace(typeof(IHttpActionInvoker), new ThrowModelStateErrorsActionInvoker());
            config.Services.Replace(
                typeof(System.Web.Http.ExceptionHandling.IExceptionHandler),
                new PolicyBasedExceptionHandler(exceptionManager, Registry.EndUserApiPolicy, req => req.GetTrackingId()));

            config.Services.Add(typeof(System.Web.Http.ExceptionHandling.IExceptionLogger), new TraceExceptionLogger());
        }

        private ExceptionManager CreateExceptionManager()
        {
            var endUserApiPolicy = new List<ExceptionPolicyEntry>
            {
                {
                    new ExceptionPolicyEntry(typeof (ModelStateValidationException),
                        PostHandlingAction.ThrowNewException,
                        new IExceptionHandler[]
                        {
                            new ModelStateValidationExceptionHandler()
                        })
                },
                {
                    new ExceptionPolicyEntry(typeof (ResourceNotFoundException),
                        PostHandlingAction.ThrowNewException,
                        new IExceptionHandler[]
                        {
                            new ToEndUserExceptionHandler(HttpStatusCode.NotFound, "Resource cannot be found.")
                        })
                },
                {
                    new ExceptionPolicyEntry(typeof (UnauthenticatedAccessException),
                        PostHandlingAction.ThrowNewException,
                        new IExceptionHandler[]
                        {
                            new ToEndUserExceptionHandler(HttpStatusCode.Unauthorized, "Authentication required.")
                        })
                },
                {
                    new ExceptionPolicyEntry(typeof (AccessDenidException),
                        PostHandlingAction.ThrowNewException,
                        new IExceptionHandler[]
                        {
                            new ToEndUserExceptionHandler(HttpStatusCode.Forbidden, "Access denied.")
                        })
                },
                {
                    new ExceptionPolicyEntry(typeof (Exception),
                        PostHandlingAction.ThrowNewException,
                        new IExceptionHandler[]
                        {
                            new UnexpectedErrorExceptionHandler()
                        })
                }
            };

            var policies = new List<ExceptionPolicyDefinition>
            {
                new ExceptionPolicyDefinition( Registry.EndUserApiPolicy, endUserApiPolicy )
            };

            var exceptionManager = new ExceptionManager(policies);

            return exceptionManager;
        }

        #endregion
   }
}
