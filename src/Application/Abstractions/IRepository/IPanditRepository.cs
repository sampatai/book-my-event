using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Application.Model;
using Domain.Pandit.Root;
using SharedKernel;

namespace Application.Abstractions.IRepository
{
    public interface IPanditRepository : IRepository<Pandit>
    {
        Task<Pandit> AddAsync(Pandit pandit, CancellationToken cancellationToken);
        Task<Pandit> UpdateAsync(Pandit pandit, CancellationToken cancellationToken);
        Task<Pandit> GetPanditAsync(Guid panditId, CancellationToken cancellationToken);
    }
    public interface IPanditReadRepository : IReadOnlyRepository<Pandit>
    {
        Task<(IEnumerable<Pandit> Pandits, int TotalCount)> ReadAsync(PanditFilter panditFilter, CancellationToken cancellationToken)
        Task<Pandit> GetPanditAsync(Guid panditId, CancellationToken cancellationToken);
        Task<bool> Exists(Guid panditId, CancellationToken cancellationToken);

    }
}
