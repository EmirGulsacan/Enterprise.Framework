namespace Enterprise.Framework.API.Endpoints;

using Enterprise.Framework.API.Common;

using Enterprise.Framework.Application.Common.Models;

using Enterprise.Framework.Application.Branches.Commands;

using Enterprise.Framework.Application.Branches.Queries;

using MediatR;

using Microsoft.AspNetCore.Mvc;



public class BranchEndpoints : IEndpointDefinition {

public void MapEndpoints(IEndpointRouteBuilder app) {
    
var group = app.MapGroup("/api/branches") {
            .WithTags("Branches") {
            .RequireAuthorization();

        group.MapPost("/", async (CreateBranchCommand command, ISender sender, HttpContext ctx) =>
        
var id = await sender.Send(command);

            return Results.Ok(ApiResponse<long>.Ok(id, traceId: ctx.TraceIdentifier));

        }
);

        group.MapGet("/", async ([AsParameters] GetBranchesQuery query, ISender sender, HttpContext ctx) =>
        
var result = await sender.Send(query);

            return Results.Ok(ApiResponse<PagedResult<BranchDto>>.Ok(result, traceId: ctx.TraceIdentifier));

        }
);

        group.MapPut("/
id:long




}
}



