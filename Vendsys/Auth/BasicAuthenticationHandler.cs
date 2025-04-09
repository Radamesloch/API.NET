using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Vendsys.Auth
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        // Private field to hold the configuration object, typically used to access app settings
        IConfiguration _configuration;

        // Constructor to initialize the handler with necessary services.
        // The 'IConfiguration' is injected to access application settings.
        public BasicAuthenticationHandler(IConfiguration configuration,
            IOptionsMonitor<AuthenticationSchemeOptions> options, // For accessing authentication options
            ILoggerFactory logger, // For logging purposes
            UrlEncoder encoder, // Used for encoding URL components
            ISystemClock timeProvider // Provides the current time for ticket expiration
            ) : base(options, logger, encoder, timeProvider) // Passing parameters to the base class constructor
        {
            _configuration = configuration; // Assigning the configuration object to the private field
        }

        // This method is called to handle authentication for an incoming request.
        // It's overridden from the base class 'AuthenticationHandler'.
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // Step 1: Check if the 'Authorization' header is present in the request.
            if (!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.Fail("Unauthorized");

            // Step 2: Retrieve the authorization header from the request.
            string authorizationHeader = Request.Headers["Authorization"];

            // Step 3: Check if the header is empty or null.
            if (string.IsNullOrEmpty(authorizationHeader))
                return AuthenticateResult.Fail("Unauthorized");

            // Step 4: Ensure the header starts with "Basic " (case insensitive).
            // This is the standard for Basic Authentication in HTTP.
            if (!authorizationHeader.StartsWith("basic ", StringComparison.OrdinalIgnoreCase))
                return AuthenticateResult.Fail("Unauthorized");

            // Step 5: Extract the token from the authorization header (the part after "Basic ").
            var token = authorizationHeader.Substring(6); // Remove "basic " from the start

            // Step 6: Decode the Base64 encoded token (username:password).
            var credentialString = Encoding.UTF8.GetString(Convert.FromBase64String(token));

            // Step 7: Split the decoded credentials into username and password.
            var credentials = credentialString.Split(":");

            // Step 8: Check if the credentials are valid (should be exactly two parts: username and password).
            if (credentials?.Length != 2)
                return AuthenticateResult.Fail("Unauthorized");

            // Step 9: Extract the username and password from the credentials.
            var username = credentials[0];
            var password = credentials[1];

            // Step 10: Validate the username and password against the configuration values.
            // These values are typically stored securely in app settings or environment variables.
            if (username != _configuration["Credentials:username"] && password != _configuration["Credentials:password"])
                return AuthenticateResult.Fail("Authentication failed");

            // Step 11: Create claims for the authenticated user.
            // Here, a simple role of "Admin" is assigned to the user.
            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, username), // Identifier claim with the username
            new Claim(ClaimTypes.Role, "Admin") // Role claim with a fixed role "Admin"
        };

            // Step 12: Create a ClaimsIdentity using the claims, and set the authentication type to "Basic".
            var identity = new ClaimsIdentity(claims, "Basic");

            // Step 13: Create a ClaimsPrincipal with the generated ClaimsIdentity.
            var claimsPrincipal = new ClaimsPrincipal(identity);

            // Step 14: Return an authentication ticket that includes the ClaimsPrincipal and the scheme name.
            // This marks the authentication as successful.
            return AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name));
        }
    }
}
