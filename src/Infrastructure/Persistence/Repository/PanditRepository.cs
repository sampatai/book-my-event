using System;
using System.Collections.Generic;
using System.Text;
using Application.Abstractions.IRepository;
using Application.Model;
using Domain.Pandit.Root;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Infrastructure.Persistence.Repository
{
    internal class PanditRepository(ILogger<PanditRepository> logger, ApplicationDbContext applicationDbContext) : IPanditRepository
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
    public class PanditReadRepository(ApplicationDbContext applicationDbContext, ILogger<PanditReadRepository> logger) : IPanditReadRepository
    {
        public async Task<Pandit> GetPanditAsync(Guid panditId, CancellationToken cancellationToken)
        {
            try
            {
                return await applicationDbContext.Pandits

                    .Include(x => x.Verifications)
                    .Include(x => x.PujaTypes)
                    .AsNoTracking()
                    .AsSplitQuery()
                    .FirstOrDefaultAsync(x => x.PanditId == panditId, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetPanditAsync {@panditId}", panditId);
                throw;
            }
        }

        public async Task<(IEnumerable<Pandit> Pandits, int TotalCount)> ReadAsync(PanditFilter panditFilter, CancellationToken cancellationToken)
        {
            try
            {
                var query = applicationDbContext.Pandits
                     .AsNoTracking()
                     .OrderBy(x => x.FullName)
                     .AsQueryable();

                var likeSearchTerm = $"%{panditFilter.SearchTerm}%";

                query = query.Where(x => EF.Functions.Like(x.FullName, likeSearchTerm) ||
                                         EF.Functions.Like(x.Languages, likeSearchTerm) ||
                                         EF.Functions.Like(x.Address.AddressLine1, likeSearchTerm) ||
                                         EF.Functions.Like(x.Address.State!, likeSearchTerm) ||
                                         EF.Functions.Like(x.Address.Street!, likeSearchTerm)

                                         );

                int totalRecords = await query.CountAsync(cancellationToken);

                var results = await query
                    .Skip((panditFilter.PageNumber - 1) * panditFilter.PageNumber)
                    .Take(panditFilter.PageNumber)
                    .ToListAsync(cancellationToken);

                return (results, totalRecords);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "{@pageNumber} {@pageSize}  {@Term}", panditFilter.PageNumber, panditFilter.PageSize, panditFilter.SearchTerm);
                throw;
            }
        }

        public async Task<bool> Exists(Guid panditId, CancellationToken cancellationToken)
        {
            return await applicationDbContext.Pandits
                .AnyAsync(p => p.PanditId == panditId, cancellationToken);
        }
    }
   
    }
