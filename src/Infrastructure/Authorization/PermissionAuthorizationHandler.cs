using Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;
using System.Security.Claims;

namespace Infrastructure.Authorization;

internal sealed class PermissionAuthorizationHandler(IServiceScopeFactory serviceScopeFactory)
    : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (context.User.Identity?.IsAuthenticated != true)
        {
            return;
        }

        using IServiceScope scope = serviceScopeFactory.CreateScope();

        PermissionProvider permissionProvider = scope.ServiceProvider.GetRequiredService<PermissionProvider>();

        HashSet<string> directPermissions = context.User
            .FindAll("permission")
            .Select(x => x.Value)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        List<string> roleNames = context.User
            .FindAll(OpenIddictConstants.Claims.Role)
            .Concat(context.User.FindAll(ClaimTypes.Role))
            .Select(x => x.Value)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        HashSet<string> rolePermissions = await permissionProvider.GetForRoleNamesAsync(roleNames);

        HashSet<string> permissions = [.. directPermissions, .. rolePermissions];

        if (permissions.Contains(requirement.Permission) || permissions.Contains("admin"))
        {
            context.Succeed(requirement);
        }
    }
}
