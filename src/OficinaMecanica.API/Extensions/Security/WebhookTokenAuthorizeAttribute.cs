using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
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

    private readonly WebhookTokenOptions _options;

    public WebhookTokenAuthorizationFilter(IOptions<WebhookTokenOptions> options)
    {
        _options = options.Value;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var expectedToken = _options.WebhookToken;

        if (!context.HttpContext.Request.Headers.TryGetValue(HeaderName, out var providedToken)
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
