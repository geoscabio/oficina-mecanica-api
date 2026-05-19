using Microsoft.EntityFrameworkCore;
using OficinaMecanica.Application.Common;

namespace OficinaMecanica.Infrastructure.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly OficinaMecanicaDbContext _dbContext;

    public UnitOfWork(OficinaMecanicaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task ExecutarEmTransacaoAsync(
        Func<CancellationToken, Task> operacao,
        CancellationToken cancellationToken = default)
    {
        var strategy = _dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            await operacao(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        });
    }
}
