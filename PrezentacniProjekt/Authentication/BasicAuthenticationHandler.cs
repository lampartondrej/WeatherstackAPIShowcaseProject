using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace PrezentacniProjekt
{
    /// <summary>
    /// Handles basic authentication by validating credentials from the Authorization header.
    /// </summary>
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BasicAuthenticationHandler"/> class.
        /// </summary>
        /// <param name="options">The monitor for the authentication scheme options.</param>
        /// <param name="logger">The factory for creating loggers.</param>
        /// <param name="encoder">The URL encoder.</param>
        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        /// <summary>
        /// Authenticates the request by parsing and validating the Basic authentication credentials from the Authorization header.
        /// </summary>
        /// <returns>
        /// An <see cref="AuthenticateResult"/> indicating whether authentication succeeded or failed.
        /// </returns>
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return Task.FromResult(AuthenticateResult.Fail("Missing Authorization Header"));
            }

            try
            {
                var authHeaderValue = Request.Headers["Authorization"].ToString();
                if (string.IsNullOrWhiteSpace(authHeaderValue))
                {
                    return Task.FromResult(AuthenticateResult.Fail("Empty Authorization Header"));
                }

                var authHeader = AuthenticationHeaderValue.Parse(authHeaderValue);
                if (string.IsNullOrEmpty(authHeader.Parameter))
                {
                    return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));
                }
                var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
                var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);
                var username = credentials[0];
                var password = credentials[1];

                // Validate credentials (replace with your own logic)
                if (IsValidUser(username, password))
                {
                    var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, username),
                new Claim(ClaimTypes.Name, username),
            };
                    var identity = new ClaimsIdentity(claims, Scheme.Name);
                    var principal = new ClaimsPrincipal(identity);
                    var ticket = new AuthenticationTicket(principal, Scheme.Name);

                    return Task.FromResult(AuthenticateResult.Success(ticket));
                }

                return Task.FromResult(AuthenticateResult.Fail("Invalid Username or Password"));
            }
            catch
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));
            }
        }

        /// <summary>
        /// Validates the provided username and password credentials.
        /// </summary>
        /// <param name="username">The username to validate.</param>
        /// <param name="password">The password to validate.</param>
        /// <returns>
        /// <c>true</c> if the credentials are valid; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// This is a simple example implementation for demonstration purposes only.
        /// Replace with actual user validation logic before using in production.
        /// </remarks>
        private bool IsValidUser(string username, string password)
        {
            // TODO: Replace with your actual user validation logic
            // This is a simple example - DO NOT use in production!
            return username == "admin" && password == "password";
        }
    }
}