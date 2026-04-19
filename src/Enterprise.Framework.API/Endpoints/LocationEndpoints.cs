namespace Enterprise.Framework.API.Endpoints;

using Enterprise.Framework.API.Common;

using Enterprise.Framework.Application.Common.Models;

using Enterprise.Framework.Application.Locations.Commands;

using Enterprise.Framework.Application.Locations.Queries;

using MediatR;



public class LocationEndpoints : IEndpointDefinition {

public void MapEndpoints(IEndpointRouteBuilder app) {
    
var group = app.MapGroup("/api/locations") {
            .WithTags("Locations") {
            .RequireAuthorization();

        group.MapGet("/tree", async (ISender sender, HttpContext ctx) =>
        
var result = await sender.Send(new GetOrganizationTreeQuery());

            return Results.Ok(ApiResponse<List<LocationTreeNodeDto>>.Ok(result, traceId: ctx.TraceIdentifier));

        }
);

        group.MapPost("/", async (CreateLocationCommand command, ISender sender, HttpContext ctx) =>
        
var result = await sender.Send(command);

            return Results.Ok(ApiResponse<long>.Ok(result, traceId: ctx.TraceIdentifier));

        }
);

    




}
}



