namespace Enterprise.Framework.API.Endpoints;

using Enterprise.Framework.API.Common;
using Enterprise.Framework.Application.Common.Models;
using Enterprise.Framework.Application.Features.Maintenances.Commands;
using Enterprise.Framework.Application.Features.Maintenances.Queries;
using Enterprise.Framework.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

public class MaintenanceEndpoints : IEndpointDefinition {
    public void MapEndpoints(IEndpointRouteBuilder app) {
        var group = app.MapGroup("/api/maintenances")
            .WithTags("Maintenances")
            .RequireAuthorization();

        group.MapGet("/", async ([AsParameters] GetMaintenancesQuery query, ISender sender) => {
            var result = await sender.Send(query);
            return Results.Ok(ApiResponse<PagedResult<MaintenanceDto>>.Ok(result));
        });

        group.MapPost("/", async (CreateMaintenanceCommand command, ISender sender) => {
            var id = await sender.Send(command);
            return Results.Ok(ApiResponse<long>.Ok(id));
        });

        group.MapPut("/{id:long}", async (long id, UpdateMaintenanceCommand command, ISender sender) => {
            if (id != command.Id) return Results.BadRequest(ApiResponse<object>.Fail("ID mismatch"));
            await sender.Send(command);
            return Results.Ok(ApiResponse<bool>.Ok(true));
        });

        group.MapDelete("/{id:long}", async (long id, ISender sender) => {
            await sender.Send(new DeleteMaintenanceCommand(id));
            return Results.Ok(ApiResponse<bool>.Ok(true));
        });
    }
}
