using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;
using Application.Command;
using Application.Model;
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
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        //// Create Pandit
        app.MapPost("pandit", async (
            CreatePandit.CreatePanditCommand request,
            ICommandHandler<CreatePandit.CreatePanditCommand> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(request, cancellationToken);
            return result.Match(Results.Created, CustomResults.Problem);
        })
        .WithTags(Tags.Pandit);
       // .RequireAuthorization();

        //// Get Pandit by Id
        app.MapGet("pandit/{id:guid}", async (
            Guid id,
            IQueryHandler<GetPandit.Query, GetPandit.Response> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetPandit.Query(id);
            var result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Pandit)
        //.RequireAuthorization()
        ;

        ////// List Pandits
        app.MapGet("pandits", static async (
            [FromQuery] int pageNumber,
            [FromQuery] int pageSize,
            [FromQuery] string? searchTerm,
            IQueryHandler<ListPandit.Query, ListPanditResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var filter = new PanditFilter(
                pageNumber,
                pageSize,
                searchTerm ?? ""
            );

            var result = await handler.Handle(new ListPandit.Query(filter), cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Pandit)
       .RequireAuthorization()
        ;

        //// Update Pandit
        app.MapPut("pandit/{id:guid}", async (
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
        //.RequireAuthorization()
        ;


    }
}