using System.Security.Claims;
using Application.Abstractions.IRepository;
using Application.Users;
using Domain.Users.Root;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.Repository;

internal sealed class UserRepository(
    UserManager<User> userManager,
    ApplicationDbContext authenticationDbContext,
    ILogger<UserRepository> logger) : IUserRepository
{
    private async Task<long?> ResolveInternalUserIdAsync(Guid userGuidId, CancellationToken cancellationToken)
    {
        return await userManager.Users
            .AsNoTracking()
            .Where(x => x.UserId == userGuidId)
            .Select(x => (long?)x.Id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Result<long>> CreateAsync(string email, string? phoneNumber, string password, CancellationToken cancellationToken)
    {
        try
        {
            var user = new User(email, phoneNumber ?? string.Empty);
            IdentityResult result = await userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                return Result.Failure<long>(UserManagementErrors.OperationFailed(string.Join("; ", result.Errors.Select(e => e.Description))));
            }

            return Result.Success(user.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Create user failed for {Email}", email);
            return Result.Failure<long>(UserManagementErrors.OperationFailed(ex.Message));
        }
    }

    public async Task<Result> UpdateAsync(long userId, string email, string? phoneNumber, long? serviceEntityId, CancellationToken cancellationToken)
    {
        try
        {
            User? user = await userManager.FindByIdAsync(userId.ToString());
            if (user is null)
            {
                return Result.Failure(UserManagementErrors.NotFound(userId));
            }

            user.Email = email;
            user.UserName = email;
            user.PhoneNumber = phoneNumber;

           

            IdentityResult result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return Result.Failure(UserManagementErrors.OperationFailed(string.Join("; ", result.Errors.Select(e => e.Description))));
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Update user failed for {UserId}", userId);
            return Result.Failure(UserManagementErrors.OperationFailed(ex.Message));
        }
    }

    public async Task<Result> DeleteAsync(long userId, CancellationToken cancellationToken)
    {
        try
        {
            User? user = await userManager.FindByIdAsync(userId.ToString());
            if (user is null)
            {
                return Result.Failure(UserManagementErrors.NotFound(userId));
            }

            IdentityResult result = await userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return Result.Failure(UserManagementErrors.OperationFailed(string.Join("; ", result.Errors.Select(e => e.Description))));
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Delete user failed for {UserId}", userId);
            return Result.Failure(UserManagementErrors.OperationFailed(ex.Message));
        }
    }

    public async Task<Result<UserDto>> GetByIdAsync(long userId, CancellationToken cancellationToken)
    {
        UserDto? user = await authenticationDbContext.Users
            .AsNoTracking()
            .Where(x => x.Id == userId)
            .Select(x => new UserDto(
                x.Id,
                x.UserId,
                x.Email,
                x.UserName,
                x.PhoneNumber
               ))
            .FirstOrDefaultAsync(cancellationToken);

        return user is null
            ? Result.Failure<UserDto>(UserManagementErrors.NotFound(userId))
            : Result.Success(user);
    }

    public async Task<Result<IReadOnlyList<UserDto>>> ListAsync(CancellationToken cancellationToken)
    {
        IReadOnlyList<UserDto> users = await authenticationDbContext.Users
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Select(x => new UserDto(
                x.Id,
                x.UserId,
                x.Email,
                x.UserName,
                x.PhoneNumber))
            .ToListAsync(cancellationToken);

        return Result.Success(users);
    }

    public async Task<Result<IReadOnlyList<UserClaimDto>>> GetClaimsAsync(long userId, CancellationToken cancellationToken)
    {
        User? user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return Result.Failure<IReadOnlyList<UserClaimDto>>(UserManagementErrors.NotFound(userId));
        }

        var claims = await userManager.GetClaimsAsync(user);

        IReadOnlyList<UserClaimDto> result = claims
            .Select(c => new UserClaimDto(c.Type, c.Value))
            .ToList();

        return Result.Success(result);
    }

    public async Task<Result> SetClaimsAsync(long userId, IReadOnlyCollection<UserClaimDto> claims, CancellationToken cancellationToken)
    {
        User? user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return Result.Failure(UserManagementErrors.NotFound(userId));
        }

        IList<Claim> existingClaims = await userManager.GetClaimsAsync(user);

        IdentityResult removeResult = await userManager.RemoveClaimsAsync(user, existingClaims);
        if (!removeResult.Succeeded)
        {
            return Result.Failure(UserManagementErrors.OperationFailed(string.Join("; ", removeResult.Errors.Select(e => e.Description))));
        }

        var newClaims = claims
            .Where(x => !string.IsNullOrWhiteSpace(x.Type) && !string.IsNullOrWhiteSpace(x.Value))
            .Select(x => new Claim(x.Type, x.Value))
            .DistinctBy(x => new { x.Type, x.Value })
            .ToList();

        IdentityResult addResult = await userManager.AddClaimsAsync(user, newClaims);
        if (!addResult.Succeeded)
        {
            return Result.Failure(UserManagementErrors.OperationFailed(string.Join("; ", addResult.Errors.Select(e => e.Description))));
        }

        return Result.Success();
    }

    public async Task<Result<IReadOnlyList<string>>> GetPermissionsAsync(Guid userGuidId, CancellationToken cancellationToken)
    {
        long? resolvedUserId = await ResolveInternalUserIdAsync(userGuidId, cancellationToken);

        if (!resolvedUserId.HasValue)
        {
            return Result.Failure<IReadOnlyList<string>>(UserManagementErrors.NotFound(0));
        }

        long userId = resolvedUserId.Value;

        User? user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return Result.Failure<IReadOnlyList<string>>(UserManagementErrors.NotFound(userId));
        }

        var userClaims = await userManager.GetClaimsAsync(user);

        List<long> roleIds = await authenticationDbContext.UserRoles
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .Select(x => x.RoleId)
            .ToListAsync(cancellationToken);

        List<string> rolePermissions = await authenticationDbContext.RoleClaims
            .AsNoTracking()
            .Where(x => roleIds.Contains(x.RoleId) && x.ClaimType == "permission")
            .Select(x => x.ClaimValue!)
            .ToListAsync(cancellationToken);

        IReadOnlyList<string> permissions = userClaims
            .Where(c => c.Type == "permission")
            .Select(c => c.Value)
            .Concat(rolePermissions)
            .Distinct()
            .OrderBy(x => x)
            .ToList();

        return Result.Success(permissions);
    }

    public async Task<Result> SetPermissionsAsync(Guid userGuidId, IReadOnlyCollection<string> permissions, CancellationToken cancellationToken)
    {
        long userId = await userManager.Users
            .AsNoTracking()
            .Where(x => x.UserId == userGuidId)
            .Select(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (userId == 0)
        {
            return Result.Failure(UserManagementErrors.NotFound(0));
        }

        User? user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return Result.Failure(UserManagementErrors.NotFound(userId));
        }

        List<long> roleIds = await authenticationDbContext.UserRoles
            .Where(x => x.UserId == userId)
            .Select(x => x.RoleId)
            .ToListAsync(cancellationToken);

        if (roleIds.Count == 0)
        {
            return Result.Failure(UserManagementErrors.InvalidInput("User has no role assigned. Assign role before setting permissions."));
        }

        List<IdentityRoleClaim<long>> existingRolePermissionClaims = await authenticationDbContext.RoleClaims
            .Where(x => roleIds.Contains(x.RoleId) && x.ClaimType == "permission")
            .ToListAsync(cancellationToken);

        authenticationDbContext.RoleClaims.RemoveRange(existingRolePermissionClaims);

        var rolePermissionClaims = roleIds
            .SelectMany(roleId => permissions
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct()
            .Select(x => new IdentityRoleClaim<long> {
                RoleId = roleId,
                ClaimType = "permission",
                ClaimValue = x
            }))
            .ToList();

        await authenticationDbContext.RoleClaims.AddRangeAsync(rolePermissionClaims, cancellationToken);
        await authenticationDbContext.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
