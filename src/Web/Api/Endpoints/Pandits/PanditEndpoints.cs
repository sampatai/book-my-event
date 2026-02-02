using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Command.Pandit;
using Application.Query.Pandit;
using Application.Abstractions.Messaging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using SharedKernel;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Pandits;

internal sealed class PanditEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        // Create Pandit
        app.MapPost("pandits", async (
            CreatePandit.Command request,
            ICommandHandler<CreatePandit.Command> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(request, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags("Pandits")
        .RequireAuthorization();

        // Get Pandit by Id
        app.MapGet("pandits/{id:guid}", async (
            Guid id,
            IQueryHandler<GetPandit.Query, GetPandit.Response> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetPandit.Query(id);
            var result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags("Pandits")
        .RequireAuthorization();

        // List Pandits
        app.MapGet("pandits", async (
            [AsParameters] ListPandit.Query query,
            IQueryHandler<ListPandit.Query, ListPandit.ListPanditResponseList> handler,
            CancellationToken cancellationToken) =>
        {
            var result = await handler.Handle(query, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags("Pandits")
        .RequireAuthorization();

        // Update Pandit
        app.MapPut("pandits/{id:guid}", async (
            Guid id,
            UpdatePandit.Command request,
            ICommandHandler<UpdatePandit.Command, UpdatePandit.UpdatePanditResponse> handler,
            CancellationToken cancellationToken) =>
        {
            // Ensure the command has the correct id
            var command = request with { PanditId = id };
            var result = await handler.Handle(command, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags("Pandits")
        .RequireAuthorization();

        // Delete Pandit
        app.MapDelete("pandits/{id:guid}", async (
            Guid id,
            ICommandHandler<DeletePandit.Command> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new DeletePandit.Command(id);
            var result = await handler.Handle(command, cancellationToken);
            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags("Pandits")
        .RequireAuthorization();
    }
}