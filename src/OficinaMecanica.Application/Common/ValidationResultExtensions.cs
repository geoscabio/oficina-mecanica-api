using FluentValidation.Results;

namespace OficinaMecanica.Application.Common;

public static class ValidationResultExtensions
{
    public static IReadOnlyCollection<string> ObterMensagensErro(this ValidationResult validationResult)
    {
        return validationResult.Errors
            .Select(error => error.ErrorMessage)
            .Where(mensagem => !string.IsNullOrWhiteSpace(mensagem))
            .Distinct()
            .ToArray();
    }
}
