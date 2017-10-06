namespace Sigma.Web.Frontend.Core.Filters
{
    using System.Globalization;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;

    public class CultureMessageHandler : DelegatingHandler
    {
        private readonly string primaryCulture;
        private readonly string[] cultures;

        public CultureMessageHandler(params string[] cultures)
        {
            this.cultures = cultures;
            this.primaryCulture = this.cultures.First();
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!this.SetHeaderIfAcceptLanguageMatchesSupportedLanguage(request))
            {
                // Whoops no localization found. Lets try Globalisation
                if (!this.SetHeaderIfGlobalAcceptLanguageMatchesSupportedLanguage(request))
                {
                    // no global or localization found
                    this.SetCulture(request, this.primaryCulture);
                }
            }

            var response = await base.SendAsync(request, cancellationToken);
            return response;
        }

        private bool SetHeaderIfAcceptLanguageMatchesSupportedLanguage(HttpRequestMessage request)
        {
            foreach (var lang in request.Headers.AcceptLanguage)
            {
                if (this.cultures.Contains(lang.Value))
                {
                    this.SetCulture(request, lang.Value);
                    return true;
                }
            }

            return false;
        }

        private bool SetHeaderIfGlobalAcceptLanguageMatchesSupportedLanguage(HttpRequestMessage request)
        {
            foreach (var lang in request.Headers.AcceptLanguage)
            {
                var globalLang = lang.Value.Substring(0, 2);
                if (this.cultures.Any(t => t.StartsWith(globalLang)))
                {
                    this.SetCulture(request, this.cultures.FirstOrDefault(i => i.StartsWith(globalLang)));
                    return true;
                }
            }

            return false;
        }

        private void SetCulture(HttpRequestMessage request, string lang)
        {
            request.Headers.AcceptLanguage.Clear();
            request.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue(lang));
            var culture = new CultureInfo(lang);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }
    }
}