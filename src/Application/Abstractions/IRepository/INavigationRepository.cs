using Domain.Navigation.Root;
using SharedKernel;

namespace Application.Abstractions.IRepository
{
    public interface INavigationRepository : IRepository<NavigationItem>
    {
        Task<NavigationItem> AddAsync(NavigationItem navigationItem, CancellationToken cancellationToken);
        Task<NavigationItem> UpdateAsync(NavigationItem navigationItem, CancellationToken cancellationToken);
        Task<NavigationItem> GetByIdAsync(long id, CancellationToken cancellationToken);
        Task RemoveAsync(NavigationItem navigationItem, CancellationToken cancellationToken);
    }

    public interface INavigationReadRepository : IReadOnlyRepository<NavigationItem>
    {
        Task<NavigationItem> GetByIdAsync(long id, CancellationToken cancellationToken);
        Task<IEnumerable<NavigationItem>> GetRootItemsAsync(CancellationToken cancellationToken);
        Task<IEnumerable<NavigationItem>> GetChildrenByParentIdAsync(long parentId, CancellationToken cancellationToken);
        Task<bool> ExistsAsync(long id, CancellationToken cancellationToken);
    }
}