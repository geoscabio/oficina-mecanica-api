using System.Text;

namespace OficinaMecanica.API.Extensions.Security;

public sealed class WebhookTokenOptions
{
    public const string SectionName = "Integracoes:Orcamento";
    public const int MinimumTokenLength = 32;

    public string WebhookToken { get; init; } = string.Empty;

    public void Validar()
    {
        if (string.IsNullOrWhiteSpace(WebhookToken))
        {
            throw new InvalidOperationException("Integracoes:Orcamento:WebhookToken nao configurado.");
        }

        if (Encoding.UTF8.GetByteCount(WebhookToken) < MinimumTokenLength)
        {
            throw new InvalidOperationException("Integracoes:Orcamento:WebhookToken deve ter pelo menos 32 caracteres.");
        }
    }
}
