using Auth.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Authorization;

internal sealed class PermissionProvider(AuthenticationDbContext authenticationDbContext)
{
    public async Task<HashSet<string>> GetForRoleNamesAsync(IReadOnlyCollection<string> roleNames)
    {
        if (roleNames.Count == 0)
        {
            return [];
        }

        List<string> normalizedRoleNames = roleNames
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.ToUpperInvariant())
            .Distinct()
            .ToList();

        List<long> roleIds = await authenticationDbContext.Roles
            .AsNoTracking()
            .Where(x => normalizedRoleNames.Contains(x.NormalizedName!))
            .Select(x => x.Id)
            .ToListAsync();

        List<string> rolePermissions = await authenticationDbContext.RoleClaims
            .AsNoTracking()
            .Where(x => roleIds.Contains(x.RoleId) && x.ClaimType == "permission")
            .Select(x => x.ClaimValue!)
            .ToListAsync();

        HashSet<string> permissionsSet = [.. rolePermissions];

        return permissionsSet;
    }
}
