using Application.Abstractions.Messaging;
using Application.Navigation.Commands;
using SharedKernel;
using Web.Api.Extensions;

namespace Web.Api.Endpoints.Navigation;

internal sealed class Update : IEndpoint
{
    public sealed class Request
    {
        public string Title { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public string? RequiredPermission { get; set; }
    }

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("navigation/{id:long}", async (
            long id,
            Request request,
            ICommandHandler<UpdateNavigationItemCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new UpdateNavigationItemCommand
            {
                Id = id,
                Title = request.Title,
                Url = request.Url,
                Icon = request.Icon,
                RequiredPermission = request.RequiredPermission
            };

            Result result = await handler.Handle(command, cancellationToken);

            return result.Match(
                onSuccess: () => Results.NoContent(),
                onFailure: Web.Api.Infrastructure.CustomResults.Problem);
        })
        .WithName("UpdateNavigationItem")
        .WithTags(Tags.Navigation)
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound)
        .RequireAuthorization("navigation.edit");
    }
}