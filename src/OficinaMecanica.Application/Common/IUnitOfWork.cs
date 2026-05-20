namespace OficinaMecanica.Application.Common;

public interface IUnitOfWork
{
    Task ExecutarEmTransacaoAsync(Func<CancellationToken, Task> operacao, CancellationToken cancellationToken = default);
}
