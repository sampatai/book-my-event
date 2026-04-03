using Application.Navigation.Queries;
using Application.Navigation.Dtos;
using SharedKernel;
using Web.Api.Extensions;
using Application.Abstractions.Messaging;

namespace Web.Api.Endpoints.Navigation;

internal sealed class List : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("navigation", async (
            IQueryHandler<GetNavigationItems, List<NavigationItemDto>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetNavigationItems();

            Result<List<NavigationItemDto>> result = await handler.Handle(query, cancellationToken);

            return result.Match(
                onSuccess: Results.Ok,
                onFailure: Web.Api.Infrastructure.CustomResults.Problem);
        })
        .WithName("ListNavigationItems")
        .WithTags(Tags.Navigation)
        .Produces<List<NavigationItemDto>>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .RequireAuthorization();
    }
}