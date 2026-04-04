using Application.Navigation.Commands;
using Application.Navigation.Queries;
using Application.Navigation.Dtos;
using SharedKernel;
using System.Security.Claims;
using Web.Api.Extensions;
using Application.Abstractions.Messaging;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints;

internal sealed class Navigation : IEndpoint
{

    public const string _NAVIGATIONS = "Navigations";

    public sealed record UpdateNavigationItemRequest(
        string Title,
        string Url,
        string? RequiredPermission,
        string? Icon);
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet($"{_NAVIGATIONS}/user/menu", GetUserMenuHandler)
            .WithName("GetUserNavigationMenu")
            .WithTags(Tags.Navigation)
            .Produces<MenuResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            //.RequireAuthorization()
            ;

        app.MapGet($"{_NAVIGATIONS}/{{id}}", GetByIdHandler)
            .WithName("GetNavigationItemById")
            .WithTags(Tags.Navigation)
            .Produces<NavigationItemDto>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        app.MapGet($"{_NAVIGATIONS}, ListHandler)
            .WithName("ListNavigationItems")
            .WithTags(Tags.Navigation)
            .Produces<List<NavigationItemDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        app.MapPost($"{_NAVIGATIONS}, CreateHandler)
            .WithName("CreateNavigationItem")
            .WithTags(Tags.Navigation)
            .Produces<long>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status409Conflict)
            //.RequireAuthorization("navigation.create")

            ;

        app.MapPut($"{_NAVIGATIONS}/{{id}}", UpdateHandler)
            .WithName("UpdateNavigationItem")
            .WithTags(Tags.Navigation)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            //.RequireAuthorization("navigation.edit")
            ;

        app.MapDelete($"{_NAVIGATIONS}/{{id}}", DeleteHandler)
            .WithName("DeleteNavigationItem")
            .WithTags(Tags.Navigation)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            //.RequireAuthorization("navigation.delete")
            ;
    }

    private async Task<IResult> GetUserMenuHandler(
        HttpContext httpContext,
        IQueryHandler<GetUserNavigationMenu, MenuResponse> handler,
        CancellationToken cancellationToken)
    {
        var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        //if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
        //{
        //    return Results.Unauthorized();
        //}

        var userPermissions = httpContext.User
            .FindAll("permission")
            .Select(c => c.Value)
            .ToList();

        var query = new GetUserNavigationMenu {
            UserId = Random.Shared.NextInt64(),
            UserPermissions = userPermissions
        };

        var result = await handler.Handle(query, cancellationToken);

        return result.Match(
            onSuccess: Results.Ok,
            onFailure: Web.Api.Infrastructure.CustomResults.Problem);
    }

    private async Task<IResult> GetByIdHandler(
        long id,
        IQueryHandler<GetNavigationItemById, NavigationItemDto> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetNavigationItemById { Id = id };
        var result = await handler.Handle(query, cancellationToken);

        return result.Match(
            onSuccess: Results.Ok,
            onFailure: CustomResults.Problem);
    }

    private async Task<IResult> ListHandler(
        IQueryHandler<GetNavigationItems, List<NavigationItemDto>> handler,
        CancellationToken cancellationToken)
    {
        var query = new GetNavigationItems();
        var result = await handler.Handle(query, cancellationToken);

        return result.Match(
            onSuccess: Results.Ok,
            onFailure: CustomResults.Problem);
    }

    private async Task<IResult> CreateHandler(
        CreateNavigationItemCommand request,
        ICommandHandler<CreateNavigationItemCommand, long> handler,
        CancellationToken cancellationToken)
    {
      

        var result = await handler.Handle(request, cancellationToken);

        return result.Match(
            onSuccess: id => Results.Created($"navigation/{id}", id),
            onFailure: CustomResults.Problem);
    }

    private async Task<IResult> UpdateHandler(
        long id,
        UpdateNavigationItemRequest request,
        ICommandHandler<UpdateNavigationItemCommand> handler,
        CancellationToken cancellationToken)
    {
        var command = new UpdateNavigationItemCommand {
            Id = id,
            Title = request.Title,
            Url = request.Url,
            Icon = request.Icon,
            RequiredPermission = request.RequiredPermission
        };

        var result = await handler.Handle(command, cancellationToken);

        return result.Match(
            onSuccess: () => Results.NoContent(),
            onFailure: CustomResults.Problem);
    }

    private async Task<IResult> DeleteHandler(
        long id,
        ICommandHandler<DeleteNavigationItemCommand> handler,
        CancellationToken cancellationToken)
    {
        var command = new DeleteNavigationItemCommand { Id = id };
        var result = await handler.Handle(command, cancellationToken);

        return result.Match(
            onSuccess: () => Results.NoContent(),
            onFailure: CustomResults.Problem);
    }


}