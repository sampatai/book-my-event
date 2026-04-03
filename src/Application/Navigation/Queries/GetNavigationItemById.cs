using Application.Abstractions.IRepository;
using Application.Abstractions.Messaging;
using Application.Navigation.Dtos;
using SharedKernel;

namespace Application.Navigation.Queries
{
    public class GetNavigationItemById : IQuery<NavigationItemDto>
    {
        public long Id { get; set; }
    }

    public class GetNavigationItemByIdHandler : IQueryHandler<GetNavigationItemById, NavigationItemDto>
    {
        private readonly INavigationReadRepository _repository;

        public GetNavigationItemByIdHandler(INavigationReadRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<NavigationItemDto>> Handle(GetNavigationItemById query, CancellationToken cancellationToken)
        {
            try
            {
                var item = await _repository.GetByIdAsync(query.Id, cancellationToken);
                if (item == null)
                    return Result<NavigationItemDto>.Failure<NavigationItemDto>(Error.NotFound("Navigation.NotFound", "Navigation item not found"));

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
                    dto.Children.Add(new NavigationItemDto
                    {
                        Id = child.Id,
                        Title = child.Title,
                        Url = child.Url,
                        Icon = child.Icon,
                        RequiredPermission = child.RequiredPermission,
                        IsAllowed = true
                    });
                }

                return Result<NavigationItemDto>.Success(dto);
            }
            catch (Exception ex)
            {
                return Result<NavigationItemDto>.Failure<NavigationItemDto>(Error.Problem("Navigation.Error", ex.Message));
            }
        }
    }
}