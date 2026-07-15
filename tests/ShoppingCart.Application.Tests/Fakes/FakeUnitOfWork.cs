using ShoppingCart.Application.Abstractions.Persistence;

namespace Application.Tests.Fakes;

internal sealed class FakeUnitOfWork
    : IUnitOfWork
{
    public int SaveChangesCalls { get; private set; }

    public Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        SaveChangesCalls++;

        return Task.FromResult(1);
    }
}