namespace Enterprise.Framework.API.Endpoints;

using Enterprise.Framework.API.Common;
using Enterprise.Framework.Application.Common.Models;
using Enterprise.Framework.Application.Employees.Commands;
using Enterprise.Framework.Application.Employees.Queries;
using Enterprise.Framework.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

public class EmployeeEndpoints : IEndpointDefinition {
    public void MapEndpoints(IEndpointRouteBuilder app) {
        var group = app.MapGroup("/api/employees")
            .WithTags("Employees")
            .RequireAuthorization();

        group.MapGet("/", async ([AsParameters] GetEmployeesQuery query, ISender sender) => {
            var result = await sender.Send(query);
            return Results.Ok(ApiResponse<PagedResult<EmployeeDto>>.Ok(result));
        });

        group.MapPost("/", async (CreateEmployeeCommand command, ISender sender) => {
            var id = await sender.Send(command);
            return Results.Ok(ApiResponse<long>.Ok(id));
        });
    }
}
