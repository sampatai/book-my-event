using System;
using System.Collections.Generic;
using System.Text;
using Application.Model;
using Domain.Pandit.Root;
using SharedKernel;

namespace Application.Abstractions.IRepository
{
    public interface IPanditRepository: IRepository<Pandit>
    {
         Task<Pandit> AddAsync(Pandit pandit, CancellationToken cancellationToken);
        Task<Pandit> UpdateAsync(Pandit pandit,CancellationToken cancellationToken);
    }
    public interface IPanditReadRepository : IReadOnlyRepository<Pandit>
    {
        Task<Pandit> ReadAsync(PanditFilter panditFilter, CancellationToken cancellationToken);
    }
}
