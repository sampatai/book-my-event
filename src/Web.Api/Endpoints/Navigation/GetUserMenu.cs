using Application.Navigation.Queries;
using Application.Navigation.Dtos;
using SharedKernel;
using System.Security.Claims;
using Web.Api.Extensions;
using Application.Abstractions.Messaging;

namespace Web.Api.Endpoints.Navigation;

internal sealed class GetUserMenu : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("navigation/user/menu", async (
            HttpContext httpContext,
            IQueryHandler<GetUserNavigationMenu, MenuResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
            {
                return Results.Unauthorized();
            }

            var userPermissions = httpContext.User
                .FindAll("permission")
                .Select(c => c.Value)
                .ToList();

            var query = new GetUserNavigationMenu
            {
                UserId = userId,
                UserPermissions = userPermissions
            };

            var result = await handler.Handle(query, cancellationToken);

            return result.Match(
                onSuccess: Results.Ok,
                onFailure: Web.Api.Infrastructure.CustomResults.Problem);
        })
        .WithName("GetUserNavigationMenu")
        .WithTags(Tags.Navigation)
        .Produces<MenuResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .RequireAuthorization();
    }
}