using Application.Abstractions.Messaging;
using Application.Users;
using Application.Users.Commands;
using Application.Users.Queries;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints;

internal sealed class Users : IEndpoint
{
    public const string Route = "users";

    public sealed record CreateUserRequest(string Email, string? PhoneNumber, string Password);
    public sealed record UpdateUserRequest(string Email, string? PhoneNumber, long? ServiceEntityId);
    public sealed record SetClaimsRequest(IReadOnlyCollection<UserClaimDto> Claims);
    public sealed record SetPermissionsRequest(IReadOnlyCollection<string> Permissions);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost(Route, async (
            CreateUserRequest request,
            ICommandHandler<CreateUser.Command, long> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(new CreateUser.Command(request.Email, request.PhoneNumber, request.Password), cancellationToken);
            return result.Match(id => Results.Created($"/api/{Route}/{id}", id), CustomResults.Problem);
        })
        .WithTags(Tags.Users)
        .RequireAuthorization();

        app.MapGet(Route, async (
            IQueryHandler<ListUsers.Query, IReadOnlyList<UserDto>> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(new ListUsers.Query(), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Users)
        .RequireAuthorization();

        app.MapGet($"{Route}/{{id:long}}", async (
            long id,
            IQueryHandler<GetUser.Query, UserDto> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(new GetUser.Query(id), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Users)
        .RequireAuthorization();

        app.MapPut($"{Route}/{{id:long}}", async (
            long id,
            UpdateUserRequest request,
            ICommandHandler<UpdateUser.Command> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(new UpdateUser.Command(id, request.Email, request.PhoneNumber, request.ServiceEntityId), cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.Users)
        .RequireAuthorization();

        app.MapDelete($"{Route}/{{id:long}}", async (
            long id,
            ICommandHandler<DeleteUser.Command> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(new DeleteUser.Command(id), cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.Users)
        .RequireAuthorization();

        app.MapGet($"{Route}/{{id:long}}/claims", async (
            long id,
            IQueryHandler<GetUserClaims.Query, IReadOnlyList<UserClaimDto>> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(new GetUserClaims.Query(id), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Users)
        .RequireAuthorization();

        app.MapPut($"{Route}/{{id:long}}/claims", async (
            long id,
            SetClaimsRequest request,
            ICommandHandler<SetUserClaims.Command> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(new SetUserClaims.Command(id, request.Claims), cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.Users)
        .RequireAuthorization();

        app.MapGet($"{Route}/{{id:long}}/permissions", async (
            long id,
            IQueryHandler<GetUserPermissions.Query, IReadOnlyList<string>> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(new GetUserPermissions.Query(id), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Users)
        .RequireAuthorization();

        app.MapPut($"{Route}/{{id:long}}/permissions", async (
            long id,
            SetPermissionsRequest request,
            ICommandHandler<SetUserPermissions.Command> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(new SetUserPermissions.Command(id, request.Permissions), cancellationToken);
            return result.Match(Results.NoContent, CustomResults.Problem);
        })
        .WithTags(Tags.Users)
        .RequireAuthorization();
    }
}
