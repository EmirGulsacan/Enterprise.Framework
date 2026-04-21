namespace Enterprise.Framework.API.Endpoints;

using Enterprise.Framework.API.Common;
using Enterprise.Framework.Application.Common.Models;
using Enterprise.Framework.Application.Features.Documents.Commands;
using Enterprise.Framework.Application.Features.Documents.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

public class DocumentEndpoints : IEndpointDefinition {
    public void MapEndpoints(IEndpointRouteBuilder app) {
        var group = app.MapGroup("/api/documents")
            .WithTags("Documents")
            .RequireAuthorization();

        group.MapGet("/", async ([AsParameters] GetDocumentsQuery query, ISender sender) => {
            var result = await sender.Send(query);
            return Results.Ok(ApiResponse<PagedResult<DocumentDto>>.Ok(result));
        }).RequireAuthorization(p => p.RequireClaim("Permission", "Documents.View"));

        group.MapPost("/", async (CreateDocumentCommand command, ISender sender) => {
            var id = await sender.Send(command);
            return Results.Ok(ApiResponse<long>.Ok(id));
        }).RequireAuthorization(p => p.RequireClaim("Permission", "Documents.Write"));

        group.MapPut("/{id:long}", async (long id, UpdateDocumentCommand command, ISender sender) => {
            if (id != command.Id) return Results.BadRequest(ApiResponse<object>.Fail("ID mismatch"));
            await sender.Send(command);
            return Results.Ok(ApiResponse<bool>.Ok(true));
        }).RequireAuthorization(p => p.RequireClaim("Permission", "Documents.Write"));

        group.MapDelete("/{id:long}", async (long id, ISender sender) => {
            await sender.Send(new DeleteDocumentCommand(id));
            return Results.Ok(ApiResponse<bool>.Ok(true));
        }).RequireAuthorization(p => p.RequireClaim("Permission", "Documents.Write"));
    }
}
