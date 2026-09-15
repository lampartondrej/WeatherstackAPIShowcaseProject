using System.Net.Http.Headers;

namespace ShowcaseProject.Web.Authentication
{
    /// <summary>
    /// Attaches the Basic authorization header to every outgoing call to the Showcase REST API.
    /// </summary>
    public sealed class BasicAuthenticationHeaderHandler : DelegatingHandler
    {
        private readonly AuthenticationHeaderValue _authorizationHeader;

        public BasicAuthenticationHeaderHandler(ApiCredentials credentials)
        {
            _authorizationHeader = new AuthenticationHeaderValue("Basic", credentials.EncodedBasicCredentials);
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Authorization = _authorizationHeader;
            return base.SendAsync(request, cancellationToken);
        }
    }
}
