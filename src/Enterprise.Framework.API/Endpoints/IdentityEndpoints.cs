namespace Enterprise.Framework.API.Endpoints;

using Enterprise.Framework.API.Common;

using Enterprise.Framework.Application.Common.Models;

using Enterprise.Framework.Application.Identity;

using Enterprise.Framework.Application.Identity.Commands;

using Enterprise.Framework.Application.Identity.Queries;

using MediatR;



public class IdentityEndpoints : IEndpointDefinition {

public void MapEndpoints(IEndpointRouteBuilder app) {
    
var group = app.MapGroup("/api/identity") {
            .WithTags("Identity") {
            .RequireAuthorization();

        group.MapGet("/me", (Keycloak.Identity.Shared.Interfaces.ICurrentUserService currentUserService, HttpContext ctx) =>
        
return Results.Ok(ApiResponse<object>.Ok(new
            
userId = currentUserService.UserId,
                localUserId = (currentUserService as dynamic).LocalUserId,
                email = currentUserService.Email,
                isAdmin = currentUserService.IsAdmin,
                permissions = currentUserService.Permissions
            }
, traceId: ctx.TraceIdentifier));

        }
);

        var usersGroup = group.MapGroup("/users");

        usersGroup.MapGet("/", async ([AsParameters] GetUsersQuery query, ISender sender, HttpContext ctx) =>
        
var result = await sender.Send(query);

            return Results.Ok(ApiResponse<PagedResult<UserDto>>.Ok(result, traceId: ctx.TraceIdentifier));

        




}
}



