using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;
using Application.Command;
using Application.Model;
using Application.Navigation.Dtos;
using Application.Query.Pandit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;
using static Application.Query.Pandit.ListPandit;


namespace Web.Api.Endpoints.Pandits;

internal sealed class PanditEndpoints : IEndpoint
{
    public const string _PANDITS = "pandits";

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        //// Create Pandit
        app.MapPost(_PANDITS, async (
            CreatePandit.CreatePanditCommand request,
            ICommandHandler<CreatePandit.CreatePanditCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(request, cancellationToken);
            return result.Match(Results.Created, CustomResults.Problem);
        })
        .WithTags(Tags.Pandit)
        .Produces<Result>(StatusCodes.Status201Created)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status409Conflict)
        .RequireAuthorization()
       ;
        //// Get Pandit by Id
        app.MapGet($"{_PANDITS}/{{id:guid}}", async (
            Guid id,
            IQueryHandler<GetPandit.Query, GetPandit.Response> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetPandit.Query(id);
            var result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Pandit)
        .Produces<GetPandit.Response>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .RequireAuthorization();


        ////// List Pandits
        app.MapGet(_PANDITS, static async (
            [AsParameters] PanditFilter filter,
            IQueryHandler<ListPandit.Query, ListPanditResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(new ListPandit.Query(filter), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Pandit)

        .Produces<Result<ListPandit.ListPanditResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
        .RequireAuthorization();
        

        //// Update Pandit
        app.MapPut($"{_PANDITS}/{{id:guid}}", async (
            Guid id,
            UpdatePandit.UpdatePanditCommand request,
            ICommandHandler<UpdatePandit.UpdatePanditCommand, UpdatePandit.UpdatePanditResponse> handler,
            CancellationToken cancellationToken) =>
        {
            // Ensure the command has the correct id
            var command = request with { PanditId = id };
            var result = await handler.Handle(command, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Pandit)
        .Produces<Result>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
        .RequireAuthorization()
        ;


    }
}