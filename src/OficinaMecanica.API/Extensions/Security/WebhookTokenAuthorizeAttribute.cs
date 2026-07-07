using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OficinaMecanica.API.Responses;
using OficinaMecanica.Application.Common;

namespace OficinaMecanica.API.Extensions.Security;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class WebhookTokenAuthorizeAttribute : TypeFilterAttribute
{
    public WebhookTokenAuthorizeAttribute()
        : base(typeof(WebhookTokenAuthorizationFilter))
    {
    }
}

internal sealed class WebhookTokenAuthorizationFilter : IAuthorizationFilter
{
    public const string HeaderName = "X-Webhook-Token";
    public const string ConfigurationKey = "Integracoes:Orcamento:WebhookToken";

    private readonly IConfiguration _configuration;

    public WebhookTokenAuthorizationFilter(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var expectedToken = _configuration[ConfigurationKey];

        if (string.IsNullOrWhiteSpace(expectedToken)
            || !context.HttpContext.Request.Headers.TryGetValue(HeaderName, out var providedToken)
            || !TokensMatch(expectedToken, providedToken.ToString()))
        {
            context.Result = new UnauthorizedObjectResult(new ErrorResponse(ApiResponseMessages.NaoAutorizado, TipoErro.NaoAutorizado));
        }
    }

    private static bool TokensMatch(string expectedToken, string providedToken)
    {
        var expectedBytes = Encoding.UTF8.GetBytes(expectedToken);
        var providedBytes = Encoding.UTF8.GetBytes(providedToken);

        return expectedBytes.Length == providedBytes.Length
            && CryptographicOperations.FixedTimeEquals(expectedBytes, providedBytes);
    }
}
