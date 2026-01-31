using System;
using System.Collections.Generic;
using System.Text;
using Application.Abstractions.IRepository;
using Application.Model;
using Domain.Pandit.Root;
using Infrastructure.Database;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Infrastructure.Persistence.Repository
{
    internal class PanditRepository (ILogger<PanditRepository> logger, ApplicationDbContext applicationDbContext) : IPanditRepository
    {
        
        
        public IUnitOfWork UnitOfWork => applicationDbContext;

        public async Task<Pandit> AddAsync(Pandit pandit, CancellationToken cancellationToken)
        {
            try
            {
                if (pandit.IsTransient())
                {
                    var entityEntry = await applicationDbContext
                                     .Pandits
                                     .AddAsync(pandit, cancellationToken);

                    return entityEntry.Entity;
                }
                else
                    return pandit;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "{@pandit}", pandit);

                throw;
            }
        }

        public async Task<Pandit> GetPanditAsync(Guid panditId, CancellationToken cancellationToken)
        {
            try
            {
                return await applicationDbContext.Pandits.AsTracking()
                    .SingleOrDefaultAsync(x => x.PanditId.Equals(panditId),
                                      
                                        cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Could not fetch Pandits {@panditId}", panditId);
                throw;
            }
        }

        public async Task<Pandit> UpdateAsync(Pandit pandit, CancellationToken cancellationToken)
        {
            try
            {
               
                return applicationDbContext
                    .Pandits
                    .Update(pandit)
                    .Entity;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Update {@pandit}", pandit);
                throw;
            }
        }
    }
    public class PanditReadRepository : IPanditReadRepository
    {
        public Task<Pandit> ReadAsync(PanditFilter panditFilter, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        
    }
}
