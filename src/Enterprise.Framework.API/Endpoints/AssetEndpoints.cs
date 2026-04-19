namespace Enterprise.Framework.API.Endpoints;

using Enterprise.Framework.API.Common;
using Enterprise.Framework.Application.Common.Models;
using Enterprise.Framework.Application.Assets.Commands;
using Enterprise.Framework.Application.Assets.Queries;
using Enterprise.Framework.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

public class AssetEndpoints : IEndpointDefinition {
    public void MapEndpoints(IEndpointRouteBuilder app) {
        var group = app.MapGroup("/api/assets")
            .WithTags("Assets")
            .RequireAuthorization();

        group.MapGet("/", async ([AsParameters] GetAssetsQuery query, ISender sender) => {
            var result = await sender.Send(query);
            return Results.Ok(ApiResponse<PagedResult<Enterprise.Framework.Application.Assets.Queries.AssetDto>>.Ok(result));
        });

        group.MapPost("/", async (CreateAssetCommand command, ISender sender) => {
            var id = await sender.Send(command);
            return Results.Ok(ApiResponse<long>.Ok(id));
        });
    }
}
