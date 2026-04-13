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

                response.AvailableActions = MapPermissionsToActionsDynamic(query.UserPermissions);
                response.User = new($"user-{query.UserId}", string.Empty, string.Empty);

                return Result<MenuResponse>.Success(response);
            }
            catch (Exception ex)
            {
                return Result<MenuResponse>.Failure<MenuResponse>(Error.Problem("Navigation.Error", ex.Message));
            }
        }

        private static bool IsUserAllowedAccess(NavigationItem item, List<string> userPermissions)
        {
            // If no permission required, allow access
            if (string.IsNullOrEmpty(item.RequiredPermission))
                return true;

            // Check if user has the required permission or admin role
            return userPermissions.Contains(item.RequiredPermission) || userPermissions.Contains("admin");
        }

        private static NavigationItemDto MapToDto(NavigationItem item, List<string> userPermissions)
        {
            var dto = new NavigationItemDto {
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
                    dto.Items.Add(MapToDto(child, userPermissions));
                }
            }

            return dto;
        }

        
        private static Dictionary<string, bool> MapPermissionsToActionsDynamic(List<string> userPermissions)
        {
            bool isAdmin = userPermissions.Contains("admin");

            // Extract all action types from permissions (e.g., "create", "edit", "delete", "list")
            var allActions = ExtractActionTypes(userPermissions);

            // Check if user has any "create" action across any resource
            bool canCreate = isAdmin || allActions.Contains("create");

            // Check if user has any "edit" action across any resource
            bool canEdit = isAdmin || allActions.Contains("edit");

            // Check if user has any "delete" action across any resource
            bool canDelete = isAdmin || allActions.Contains("delete");

            // Check if user has any "list" action across any resource
            bool canList = isAdmin || allActions.Contains("list");

            return new Dictionary<string, bool>
            {
                { "CanView", true },  // Everyone can view the menu
                { "CanCreate", canCreate },
                { "CanEdit", canEdit },
                { "CanDelete", canDelete },
                { "CanList", canList }
            };
        }

        /// <summary>
        /// Extracts action types from permission strings.
        /// Example: ["users.create", "pandit.edit", "devotee.list"] 
        /// Returns: ["create", "edit", "list"]
        /// </summary>
        private static HashSet<string> ExtractActionTypes(List<string> userPermissions)
        {
            var actions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var permission in userPermissions)
            {
                // Skip "admin" as it's not a resource.action format
                if (permission.Equals("admin", StringComparison.OrdinalIgnoreCase))
                    continue;

                // Split permission on dot (e.g., "users.create" -> ["users", "create"])
                var parts = permission.Split('.');

                // If valid permission format (resource.action)
                if (parts.Length >= 2)
                {
                    // Add the action part (everything after the first dot)
                    var action = string.Join(".", parts.Skip(1));
                    actions.Add(action);
                }
            }

            return actions;
        }
    }
}