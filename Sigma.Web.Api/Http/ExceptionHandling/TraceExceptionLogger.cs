namespace Sigma.Web.Http.ExceptionHandling
{
    /// <summary>
    /// Logs exceptions
    /// </summary>
    public class TraceExceptionLogger : System.Web.Http.ExceptionHandling.ExceptionLogger
    {
        public override void Log(System.Web.Http.ExceptionHandling.ExceptionLoggerContext context)
        {
            System.Diagnostics.Trace.TraceError(context.ExceptionContext.Exception.ToString());
        }
    }
}