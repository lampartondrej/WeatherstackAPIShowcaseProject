using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace ShowcaseProject
{
    /// <summary>
    /// Handles basic authentication by validating credentials from the Authorization header.
    /// Credentials are retrieved from environment variables for security.
    /// </summary>
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IConfiguration _configuration;
        private readonly string _validUsername;
        private readonly string _validPassword;

        public BasicAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            IConfiguration configuration)
            : base(options, logger, encoder)
        {
            _configuration = configuration;
            
            // Get environment variable names from configuration
            var usernameEnvVar = _configuration.GetValue<string>("AuthSettings:UsernameEnvVar") 
                ?? throw new InvalidOperationException("AuthSettings:UsernameEnvVar is not set in configuration.");
            var passwordEnvVar = _configuration.GetValue<string>("AuthSettings:PasswordEnvVar") 
                ?? throw new InvalidOperationException("AuthSettings:PasswordEnvVar is not set in configuration.");
            
            // Get actual credentials from environment variables
            _validUsername = Environment.GetEnvironmentVariable(usernameEnvVar) 
                ?? throw new InvalidOperationException($"Environment variable '{usernameEnvVar}' is not set.");
            _validPassword = Environment.GetEnvironmentVariable(passwordEnvVar) 
                ?? throw new InvalidOperationException($"Environment variable '{passwordEnvVar}' is not set.");
        }

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

        private bool IsValidUser(string username, string password)
        {
            return username == _validUsername && password == _validPassword;
        }
    }
}