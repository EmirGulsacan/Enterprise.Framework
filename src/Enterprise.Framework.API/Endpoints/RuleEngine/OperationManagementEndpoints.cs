namespace Enterprise.Framework.API.Endpoints.RuleEngine;

using Enterprise.Framework.API.Common;
using Enterprise.Framework.Application.RuleEngine.Commands;
using Enterprise.Framework.Application.RuleEngine.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

public class OperationManagementEndpoints : IEndpointDefinition
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/operations")
            .WithTags("OperationManagement");

        group.MapGet("/", async (ISender sender) =>
        {
            var result = await sender.Send(new GetOperationDefinitionsQuery());
            return Results.Ok(result);
        });

        group.MapPost("/", async ([FromBody] CreateOperationDefinitionCommand command, ISender sender) =>
        {
            var id = await sender.Send(command);
            return Results.Ok(id);
        });

        group.MapPut("/{code}", async (string code, [FromBody] UpdateOperationDefinitionCommand command, ISender sender) =>
        {
            if (code != command.Code)
                return Results.BadRequest();

            await sender.Send(command);
            return Results.NoContent();
        });

        group.MapDelete("/{code}", async (string code, ISender sender) =>
        {
            await sender.Send(new DeleteOperationDefinitionCommand(code));
            return Results.NoContent();
        });
    }
}
