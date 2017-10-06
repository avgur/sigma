namespace Sigma.Web.Frontend.Core.Filters
{
    using System.Web.Http;
    using System;
    using System.Web.Http.Controllers;
    using System.Diagnostics.Contracts;
    using Sigma.Web.Http.Exceptions;

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class ApiAuthorizeAttribute : AuthorizeAttribute
    {
        protected override void HandleUnauthorizedRequest(HttpActionContext actionContext)
        {
            Contract.Assert(actionContext != null);
            var principal = actionContext.RequestContext.Principal;
            var authenticated = principal != null && principal.Identity.IsAuthenticated;
            if (authenticated)
            {
                throw new AccessDenidException();
            }
            else
            {
                throw new UnauthenticatedAccessException();
            }
        }
    }
}