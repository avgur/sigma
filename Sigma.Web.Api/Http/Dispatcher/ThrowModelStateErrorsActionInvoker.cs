namespace Sigma.Web.Http.Dispatcher
{
    using Sigma.Web.Http.Exceptions;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Http.Controllers;

    /// <summary>
    /// Handles binding exceptions (f.e,. when action expects int id, but the client provides string)
    /// </summary>
    public class ThrowModelStateErrorsActionInvoker : ApiControllerActionInvoker
    {
        public override Task<HttpResponseMessage> InvokeActionAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            if (!actionContext.ModelState.IsValid)
            {
                throw new ModelStateValidationException(actionContext.ModelState);
            }

            return base.InvokeActionAsync(actionContext, cancellationToken);
        }
    }
}