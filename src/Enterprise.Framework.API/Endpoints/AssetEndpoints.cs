namespace Enterprise.Framework.API.Endpoints;

using Enterprise.Framework.API.Common;
using Enterprise.Framework.Application.Common.Models;
using Enterprise.Framework.Application.Features.Assets.Commands;
using Enterprise.Framework.Application.Features.Assets.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

public class AssetEndpoints : IEndpointDefinition {
    public void MapEndpoints(IEndpointRouteBuilder app) {
        var group = app.MapGroup("/api/assets")
            .WithTags("Assets")
            .RequireAuthorization();

        group.MapGet("/", async ([AsParameters] GetAssetsQuery query, ISender sender) => {
            var result = await sender.Send(query);
            return Results.Ok(ApiResponse<PagedResult<Enterprise.Framework.Application.Features.Assets.Queries.AssetDto>>.Ok(result));
        }).RequireAuthorization(p => p.RequireClaim("Permission", "Assets.View"));

        group.MapPost("/", async (CreateAssetCommand command, ISender sender) => {
            var id = await sender.Send(command);
            return Results.Ok(ApiResponse<long>.Ok(id));
        }).RequireAuthorization(p => p.RequireClaim("Permission", "Assets.Write"));

        group.MapPut("/{id:long}", async (long id, UpdateAssetCommand command, ISender sender) => {
            if (id != command.Id) return Results.BadRequest(ApiResponse<object>.Fail("ID mismatch"));
            await sender.Send(command);
            return Results.Ok(ApiResponse<bool>.Ok(true));
        }).RequireAuthorization(p => p.RequireClaim("Permission", "Assets.Write"));

        group.MapDelete("/{id:long}", async (long id, ISender sender) => {
            await sender.Send(new DeleteAssetCommand(id));
            return Results.Ok(ApiResponse<bool>.Ok(true));
        }).RequireAuthorization(p => p.RequireClaim("Permission", "Assets.Write"));
    }
}
