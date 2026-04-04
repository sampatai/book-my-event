using Application.Abstractions.IRepository;
using Domain.Navigation.Root;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel;

namespace Infrastructure.Persistence.Repository
{
    internal class NavigationRepository(ILogger<NavigationRepository> logger, ApplicationDbContext applicationDbContext) : INavigationRepository
    {
        public IUnitOfWork UnitOfWork => applicationDbContext;

        public async Task<NavigationItem> AddAsync(NavigationItem navigationItem, CancellationToken cancellationToken)
        {
            try
            {
                if (navigationItem.IsTransient())
                {
                    var entityEntry = await applicationDbContext
                        .NavigationItems
                        .AddAsync(navigationItem, cancellationToken);

                    return entityEntry.Entity;
                }
                else
                    return navigationItem;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "{@navigationItem}", navigationItem);
                throw;
            }
        }

        public async Task<NavigationItem> GetByIdAsync(long id, CancellationToken cancellationToken)
        {
            try
            {
                return await applicationDbContext.NavigationItems
                    .AsTracking()
                    .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Could not fetch NavigationItem {@id}", id);
                throw;
            }
        }

        public async Task<NavigationItem> UpdateAsync(NavigationItem navigationItem, CancellationToken cancellationToken)
        {
            try
            {
                return applicationDbContext
                    .NavigationItems
                    .Update(navigationItem)
                    .Entity;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Update {@navigationItem}", navigationItem);
                throw;
            }
        }

        public async Task RemoveAsync(NavigationItem navigationItem, CancellationToken cancellationToken)
        {
            try
            {
                applicationDbContext.NavigationItems.Remove(navigationItem);
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Remove {@navigationItem}", navigationItem);
                throw;
            }
        }
    }

    public class NavigationReadRepository(ApplicationDbContext applicationDbContext, ILogger<NavigationReadRepository> logger) : INavigationReadRepository
    {
        public async Task<NavigationItem> GetByIdAsync(long id, CancellationToken cancellationToken)
        {
            try
            {
                return await applicationDbContext.NavigationItems
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetByIdAsync {@id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<NavigationItem>> GetRootItemsAsync(CancellationToken cancellationToken)
        {
            try
            {
                return await applicationDbContext.NavigationItems
                    .AsNoTracking()
                    .Where(x => !x.ParentId.HasValue)
                    .OrderBy(x => x.Title)
                    .ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetRootItemsAsync failed");
                throw;
            }
        }

        public async Task<IEnumerable<NavigationItem>> GetChildrenByParentIdAsync(long parentId, CancellationToken cancellationToken)
        {
            try
            {
                return await applicationDbContext.NavigationItems
                    .AsNoTracking()
                    .Where(x => x.ParentId == parentId)
                    .OrderBy(x => x.Title)
                    .ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetChildrenByParentIdAsync {@parentId}", parentId);
                throw;
            }
        }

        public async Task<bool> ExistsAsync(long id, CancellationToken cancellationToken)
        {
            try
            {
                return await applicationDbContext.NavigationItems
                    .AnyAsync(x => x.Id == id, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "ExistsAsync {@id}", id);
                throw;
            }
        }
    }
}