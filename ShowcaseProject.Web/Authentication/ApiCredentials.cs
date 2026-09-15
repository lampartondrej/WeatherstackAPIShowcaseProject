using System.Text;

namespace ShowcaseProject.Web.Authentication
{
    /// <summary>
    /// Credentials used to call the Showcase REST API. Resolved once during startup so a
    /// misconfigured environment fails immediately instead of on every incoming request.
    /// </summary>
    public sealed class ApiCredentials
    {
        private ApiCredentials(string username, string password)
        {
            EncodedBasicCredentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
        }

        /// <summary>Base64 encoded "username:password" value for the Basic authorization header.</summary>
        public string EncodedBasicCredentials { get; }

        public static ApiCredentials FromConfiguration(IConfiguration configuration)
        {
            var usernameEnvVar = configuration.GetValue<string>("AuthSettings:UsernameEnvVar")
                ?? throw new InvalidOperationException("AuthSettings:UsernameEnvVar is not set in configuration.");
            var passwordEnvVar = configuration.GetValue<string>("AuthSettings:PasswordEnvVar")
                ?? throw new InvalidOperationException("AuthSettings:PasswordEnvVar is not set in configuration.");

            var username = Environment.GetEnvironmentVariable(usernameEnvVar)
                ?? throw new InvalidOperationException($"Environment variable '{usernameEnvVar}' is not set.");
            var password = Environment.GetEnvironmentVariable(passwordEnvVar)
                ?? throw new InvalidOperationException($"Environment variable '{passwordEnvVar}' is not set.");

            return new ApiCredentials(username, password);
        }
    }
}
