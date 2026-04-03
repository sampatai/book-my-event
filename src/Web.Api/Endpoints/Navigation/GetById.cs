using Application.Navigation.Queries;
using Application.Navigation.Dtos;
using SharedKernel;
using Web.Api.Extensions;
using Application.Abstractions.Messaging;

namespace Web.Api.Endpoints.Navigation;

internal sealed class GetById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("navigation/{id:long}", async (
            long id,
            IQueryHandler<GetNavigationItemById, NavigationItemDto> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetNavigationItemById { Id = id };

            Result<NavigationItemDto> result = await handler.Handle(query, cancellationToken);

            return result.Match(
                onSuccess: Results.Ok,
                onFailure: Web.Api.Infrastructure.CustomResults.Problem);
        })
        .WithName("GetNavigationItemById")
        .WithTags(Tags.Navigation)
        .Produces<NavigationItemDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound)
        .RequireAuthorization();
    }
}