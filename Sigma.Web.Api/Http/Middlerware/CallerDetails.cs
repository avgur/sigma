using System;

namespace Sigma.Web.Frontend.Core.DataHttp.Middleware
{
    public class CallerDetails
    {
        public string TrackingId { get; set; }

        public string RemoteIpAddress { get; set; }

        public string LocalIpAddress { get; set; }

        public string UserAgent { get; set; }

        public DateTime OperationDate { get; set; }

        public string Method { get; set; }

        public string Uri { get; set; }
    }
}