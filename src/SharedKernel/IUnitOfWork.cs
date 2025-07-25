﻿namespace SharedKernel;

public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken);
}
