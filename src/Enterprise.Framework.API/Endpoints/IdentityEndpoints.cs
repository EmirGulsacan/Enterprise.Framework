namespace Enterprise.Framework.API.Endpoints;

using Enterprise.Framework.API.Common;
using Enterprise.Framework.Application.Common.Models;
using Enterprise.Framework.Application.Features.Identity.Commands;
using Enterprise.Framework.Application.Features.Identity.Queries;
using Enterprise.Framework.Application.Features.Identity;
using MediatR;
using Microsoft.AspNetCore.Mvc;

public class IdentityEndpoints : IEndpointDefinition {
    public void MapEndpoints(IEndpointRouteBuilder app) {
        var group = app.MapGroup("/api/identity")
            .WithTags("Identity")
            .RequireAuthorization();

        group.MapGet("/me", (Keycloak.Identity.Shared.Interfaces.ICurrentUserService currentUserService, HttpContext ctx) => {
            return Results.Ok(ApiResponse<object>.Ok(new {
                userId = currentUserService.UserId,
                localUserId = (currentUserService as dynamic).LocalUserId,
                email = currentUserService.Email,
                isAdmin = currentUserService.IsAdmin,
                permissions = currentUserService.Permissions
            }, traceId: ctx.TraceIdentifier));
        });

        var usersGroup = group.MapGroup("/users");

        usersGroup.MapGet("/", async ([AsParameters] GetUsersQuery query, ISender sender, HttpContext ctx) => {
            var result = await sender.Send(query);
            return Results.Ok(ApiResponse<PagedResult<Enterprise.Framework.Application.Features.Identity.Queries.UserDto>>.Ok(result, traceId: ctx.TraceIdentifier));
        });

        usersGroup.MapPost("/", async (CreateUserCommand command, ISender sender, HttpContext ctx) => {
            var id = await sender.Send(command);
            return Results.Ok(ApiResponse<long>.Ok(id, "Kullanıcı başarıyla oluşturuldu.", traceId: ctx.TraceIdentifier));
        });

        usersGroup.MapPut("/{id:long}", async (long id, UpdateUserCommand command, ISender sender, HttpContext ctx) => {
            if (id != command.Id) return Results.BadRequest(ApiResponse<object>.Fail("Geçersiz kullanıcı ID'si.", traceId: ctx.TraceIdentifier));
            await sender.Send(command);
            return Results.Ok(ApiResponse<bool>.Ok(true, "Kullanıcı başarıyla güncellendi.", traceId: ctx.TraceIdentifier));
        });

        usersGroup.MapDelete("/{id:long}", async (long id, ISender sender, HttpContext ctx) => {
            await sender.Send(new DeleteUserCommand(id));
            return Results.Ok(ApiResponse<bool>.Ok(true, "Kullanıcı başarıyla silindi.", traceId: ctx.TraceIdentifier));
        });

        usersGroup.MapPost("/{id:long}/roles", async (long id, [FromBody] List<long> roleIds, ISender sender, HttpContext ctx) => {
            await sender.Send(new UpdateUserRolesCommand(id, roleIds));
            return Results.Ok(ApiResponse<bool>.Ok(true, "Kullanıcı rolleri başarıyla güncellendi.", traceId: ctx.TraceIdentifier));
        });

        var rolesGroup = group.MapGroup("/roles");

        rolesGroup.MapGet("/", async (ISender sender, HttpContext ctx) => {
            var result = await sender.Send(new GetRolesQuery());
            return Results.Ok(ApiResponse<List<RoleDto>>.Ok(result, traceId: ctx.TraceIdentifier));
        });

        rolesGroup.MapPost("/", async (CreateRoleCommand command, ISender sender, HttpContext ctx) => {
            var result = await sender.Send(command);
            return Results.Ok(ApiResponse<long>.Ok(result, "Rol başarıyla oluşturuldu.", traceId: ctx.TraceIdentifier));
        });

        rolesGroup.MapGet("/{id}/permissions", async (long id, ISender sender, HttpContext ctx) => {
            var result = await sender.Send(new GetRoleWithPermissionsQuery(id));
            return Results.Ok(ApiResponse<RoleWithPermissionsDto>.Ok(result, traceId: ctx.TraceIdentifier));
        });

        rolesGroup.MapPut("/{id}/permissions", async (long id, UpdateRolePermissionsCommand command, ISender sender, HttpContext ctx) => {
            if (id != command.RoleId) return Results.BadRequest(ApiResponse<object>.Fail("Geçersiz rol ID'si.", traceId: ctx.TraceIdentifier));
            await sender.Send(command);
            return Results.Ok(ApiResponse<bool>.Ok(true, "Yetkiler başarıyla güncellendi.", traceId: ctx.TraceIdentifier));
        });

        var permissionsGroup = group.MapGroup("/permissions");

        permissionsGroup.MapGet("/", async (ISender sender, HttpContext ctx) => {
            var result = await sender.Send(new GetPermissionsQuery());
            return Results.Ok(ApiResponse<List<PermissionDto>>.Ok(result, traceId: ctx.TraceIdentifier));
        });
    }
}
