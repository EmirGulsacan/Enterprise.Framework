namespace Enterprise.Framework.API.Endpoints;

using Enterprise.Framework.API.Common;
using Enterprise.Framework.Application.Common.Models;
using Enterprise.Framework.Application.Documents.Commands;
using Enterprise.Framework.Application.Documents.Queries;
using Enterprise.Framework.Domain.Entities;
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
        });

        group.MapPost("/", async (CreateDocumentCommand command, ISender sender) => {
            var id = await sender.Send(command);
            return Results.Ok(ApiResponse<long>.Ok(id));
        });
    }
}
