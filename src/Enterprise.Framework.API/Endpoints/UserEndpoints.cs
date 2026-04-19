namespace Enterprise.Framework.API.Endpoints;

using Enterprise.Framework.API.Common;

using Enterprise.Framework.Application.Identity.Commands;

using Enterprise.Framework.Application.Identity.Queries;

using MediatR;

using Microsoft.AspNetCore.Mvc;



public static class UserEndpoints {

public static void MapUserEndpoints(this IEndpointRouteBuilder app) {
    
var group = app.MapGroup("/api/users") {
            .WithTags("Users") {
            .RequireAuthorization();

        group.MapGet("/", async (IMediator mediator, [AsParameters] GetUsersQuery query) =>
        
var result = await mediator.Send(query);

            return Results.Ok(ApiResponse<object>.Ok(result));

        }
);

        group.MapPost("/", async (IMediator mediator, CreateUserCommand command) =>
        
var id = await mediator.Send(command);

            return Results.Ok(ApiResponse<long>.Ok(id));

        }
).RequireAuthorization(p => p.RequireClaim("Permission", "Identity.Users.Write"));

        group.MapPut("/
id




}
}



