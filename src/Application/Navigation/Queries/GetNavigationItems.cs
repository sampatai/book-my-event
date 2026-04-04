using Application.Abstractions.IRepository;
using Application.Abstractions.Messaging;
using Application.Navigation.Dtos;
using SharedKernel;

namespace Application.Navigation.Queries
{
    public class GetNavigationItems : IQuery<List<NavigationItemDto>>
    {
    }

    public class GetNavigationItemsHandler : IQueryHandler<GetNavigationItems, List<NavigationItemDto>>
    {
        private readonly INavigationReadRepository _repository;

        public GetNavigationItemsHandler(INavigationReadRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<List<NavigationItemDto>>> Handle(GetNavigationItems query, CancellationToken cancellationToken)
        {
            try
            {
                var rootItems = await _repository.GetRootItemsAsync(cancellationToken);

                var dtos = new List<NavigationItemDto>();

                foreach (var item in rootItems)
                {
                    dtos.Add(MapToDto(item));
                }

                return Result<List<NavigationItemDto>>.Success(dtos);
            }
            catch (Exception ex)
            {
                return Result<List<NavigationItemDto>>.Failure<List<NavigationItemDto>>(Error.Problem("Navigation.Error", ex.Message));
            }
        }

        private static NavigationItemDto MapToDto(Domain.Navigation.Root.NavigationItem item)
        {
            var dto = new NavigationItemDto
            {
                Id = item.Id,
                Title = item.Title,
                Url = item.Url,
                Icon = item.Icon,
                RequiredPermission = item.RequiredPermission,
                IsAllowed = true
            };

            foreach (var child in item.Children)
            {
                dto.Items.Add(new NavigationItemDto
                {
                    Id = child.Id,
                    Title = child.Title,
                    Url = child.Url,
                    Icon = child.Icon,
                    RequiredPermission = child.RequiredPermission,
                    IsAllowed = true
                });
            }

            return dto;
        }
    }
}