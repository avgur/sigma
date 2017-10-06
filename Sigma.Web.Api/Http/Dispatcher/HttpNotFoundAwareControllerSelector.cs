namespace Sigma.Web.Http.Dispatcher
{
    using System.Web.Http;

    using System.Net.Http;
    using System.Net;
    using System.Web.Http.Controllers;
    using System.Web.Http.Dispatcher;
    using Sigma.Web.Http.Exceptions;

    /// <summary>
    /// Handles missing controller
    /// </summary>
    public class HttpNotFoundAwareControllerSelector : DefaultHttpControllerSelector
    {
        public HttpNotFoundAwareControllerSelector(HttpConfiguration configuration)
            : base(configuration)
        {
        }

        public override HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            HttpControllerDescriptor decriptor = null;
            try
            {
                decriptor = base.SelectController(request);
            }
            catch (HttpResponseException ex)
            {
                var code = ex.Response.StatusCode;
                if (code != HttpStatusCode.NotFound)
                {
                    throw;
                }

                throw new ResourceNotFoundException("Web api controller cannot be found", ex);
            }

            return decriptor;
        }
    }

}