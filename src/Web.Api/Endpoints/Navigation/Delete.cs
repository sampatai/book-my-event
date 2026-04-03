using Application.Abstractions.Messaging;
using Application.Navigation.Commands;
using SharedKernel;
using Web.Api.Extensions;

namespace Web.Api.Endpoints.Navigation;

internal sealed class Delete : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("navigation/{id:long}", async (
            long id,
            ICommandHandler<DeleteNavigationItemCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new DeleteNavigationItemCommand { Id = id };

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(
                onSuccess: () => Results.NoContent(),
                onFailure: Web.Api.Infrastructure.CustomResults.Problem);
        })
        .WithName("DeleteNavigationItem")
        .WithTags(Tags.Navigation)
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound)
        .RequireAuthorization("navigation.delete");
    }
}