using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;
using TaskBoard.Api.Services.Interface;

namespace TaskBoard.Api.Handler
{
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IApiKeyService _apiKeyService;
        private const string API_KEY_HEADER = "X-API-KEY";

        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            IApiKeyService apiKeyService) : base(options, logger, encoder)
        {
            _apiKeyService = apiKeyService;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue(API_KEY_HEADER, out var apiKeyHeaderValues))
                return AuthenticateResult.Fail("API Key missing");

            string? providedApiKey = apiKeyHeaderValues.FirstOrDefault();

            if (string.IsNullOrWhiteSpace(providedApiKey))
                return AuthenticateResult.Fail("Invalid API Key");

            bool isValid = await _apiKeyService.IsValid(providedApiKey);
            if (!isValid)
                return AuthenticateResult.Fail("Invalid API Key");

            var claims = new[] { new Claim(ClaimTypes.Name, "APIClient") };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}
