using Application.Abstractions.Messaging;
using Application.Navigation.Commands;
using SharedKernel;
using Web.Api.Extensions;

namespace Web.Api.Endpoints.Navigation;

internal sealed class Create : IEndpoint
{
    public sealed class Request
    {
        public string Title { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string? Icon { get; set; }
        public string? RequiredPermission { get; set; }
        public long? ParentId { get; set; }
    }

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("navigation", async (
            Request request,
            ICommandHandler<CreateNavigationItemCommand, long> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateNavigationItemCommand
            {
                Title = request.Title,
                Url = request.Url,
                Icon = request.Icon,
                RequiredPermission = request.RequiredPermission,
                ParentId = request.ParentId
            };

            Result<long> result = await handler.Handle(command, cancellationToken);

            return result.Match(
                onSuccess: (id) => Results.Created($"navigation/{id}", new { id }),
                onFailure: Web.Api.Infrastructure.CustomResults.Problem);
        })
        .WithName("CreateNavigationItem")
        .WithTags(Tags.Navigation)
        .Produces<object>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .RequireAuthorization("navigation.create");
    }
}