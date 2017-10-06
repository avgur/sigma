namespace Sigma.Web.Http.Dispatcher
{
    using System.Web.Http;
    using System.Net;
    using System.Web.Http.Controllers;
    using Sigma.Web.Http.Exceptions;

    /// <summary>
    /// Handles missing controller action
    /// </summary>
    public class HttpNotFoundAwareControllerActionSelector : ApiControllerActionSelector
    {
        public HttpNotFoundAwareControllerActionSelector()
        {
        }

        public override HttpActionDescriptor SelectAction(HttpControllerContext controllerContext)
        {
            HttpActionDescriptor decriptor = null;
            try
            {
                decriptor = base.SelectAction(controllerContext);
            }
            catch (HttpResponseException ex)
            {
                var code = ex.Response.StatusCode;
                if (code != HttpStatusCode.NotFound && code != HttpStatusCode.MethodNotAllowed)
                {
                    throw;
                }

                throw new ResourceNotFoundException("Web api controller action cannot be found", ex);
            }

            return decriptor;
        }
    }
}