namespace Enterprise.Framework.API.Endpoints;

using Enterprise.Framework.API.Common;
using Enterprise.Framework.Application.Common.Models;
using Enterprise.Framework.Application.Features.Labors.Commands;
using Enterprise.Framework.Application.Features.Labors.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

public class LaborEndpoints : IEndpointDefinition {
    public void MapEndpoints(IEndpointRouteBuilder app) {
        var group = app.MapGroup("/api/labors")
            .WithTags("Labors")
            .RequireAuthorization();

        group.MapGet("/", async ([AsParameters] GetLaborsQuery query, ISender sender) => {
            var result = await sender.Send(query);
            return Results.Ok(ApiResponse<PagedResult<LaborDto>>.Ok(result));
        }).RequireAuthorization(p => p.RequireClaim("Permission", "Labors.View"));

        group.MapPost("/", async (CreateLaborCommand command, ISender sender) => {
            var id = await sender.Send(command);
            return Results.Ok(ApiResponse<long>.Ok(id));
        }).RequireAuthorization(p => p.RequireClaim("Permission", "Labors.Write"));

        group.MapPut("/{id:long}", async (long id, UpdateLaborCommand command, ISender sender) => {
            if (id != command.Id) return Results.BadRequest(ApiResponse<object>.Fail("ID mismatch"));
            await sender.Send(command);
            return Results.Ok(ApiResponse<bool>.Ok(true));
        }).RequireAuthorization(p => p.RequireClaim("Permission", "Labors.Write"));

        group.MapDelete("/{id:long}", async (long id, ISender sender) => {
            await sender.Send(new DeleteLaborCommand(id));
            return Results.Ok(ApiResponse<bool>.Ok(true));
        }).RequireAuthorization(p => p.RequireClaim("Permission", "Labors.Write"));
    }
}
