using Application.Abstractions.IRepository;
using Application.Abstractions.Messaging;
using Application.Navigation.Dtos;
using Domain.Navigation.Root;
using SharedKernel;

namespace Application.Navigation.Queries
{
    public class GetUserNavigationMenu : IQuery<MenuResponse>
    {
        public long UserId { get; set; }
        public List<string> UserPermissions { get; set; } = new();
    }

    public class GetUserNavigationMenuHandler : IQueryHandler<GetUserNavigationMenu, MenuResponse>
    {
        private readonly INavigationReadRepository _navigationRepository;

        public GetUserNavigationMenuHandler(INavigationReadRepository navigationRepository)
        {
            _navigationRepository = navigationRepository;
        }

        public async Task<Result<MenuResponse>> Handle(GetUserNavigationMenu query, CancellationToken cancellationToken)
        {
            try
            {
                var rootItems = await _navigationRepository.GetRootItemsAsync(cancellationToken);

                var response = new MenuResponse();

                foreach (var item in rootItems)
                {
                    var isAllowed = IsUserAllowedAccess(item, query.UserPermissions);

                    if (isAllowed)
                    {
                        var dto = MapToDto(item, query.UserPermissions);
                        response.MenuItems.Add(dto);
                    }
                }

                response.AvailableActions = MapPermissionsToActions(query.UserPermissions);

                return Result<MenuResponse>.Success(response);
            }
            catch (Exception ex)
            {
                return Result<MenuResponse>.Failure<MenuResponse>(Error.Problem("Navigation.Error", ex.Message));
            }
        }

        private static bool IsUserAllowedAccess(NavigationItem item, List<string> userPermissions)
        {
            if (string.IsNullOrEmpty(item.RequiredPermission))
                return true;

            return userPermissions.Contains(item.RequiredPermission);
        }

        private static NavigationItemDto MapToDto(NavigationItem item, List<string> userPermissions)
        {
            var dto = new NavigationItemDto
            {
                Id = item.Id,
                Title = item.Title,
                Url = item.Url,
                Icon = item.Icon,
                RequiredPermission = item.RequiredPermission,
                IsAllowed = IsUserAllowedAccess(item, userPermissions)
            };

            foreach (var child in item.Children)
            {
                if (IsUserAllowedAccess(child, userPermissions))
                {
                    dto.Children.Add(MapToDto(child, userPermissions));
                }
            }

            return dto;
        }

        private static Dictionary<string, bool> MapPermissionsToActions(List<string> userPermissions)
        {
            return new Dictionary<string, bool>
            {
                { "CanView", true },
                { "CanCreate", userPermissions.Contains("navigation.create") || userPermissions.Contains("admin") },
                { "CanEdit", userPermissions.Contains("navigation.edit") || userPermissions.Contains("admin") },
                { "CanDelete", userPermissions.Contains("navigation.delete") || userPermissions.Contains("admin") },
                { "CanList", userPermissions.Contains("navigation.list") || userPermissions.Contains("admin") }
            };
        }
    }
}