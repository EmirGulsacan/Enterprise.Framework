namespace Enterprise.Framework.API.Endpoints.RuleEngine;

using Enterprise.Framework.API.Common;
using Enterprise.Framework.Application.RuleEngine.Commands;
using Enterprise.Framework.Application.RuleEngine.Queries;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

public class RuleDefinitionEndpoints : IEndpointDefinition
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/rule-definitions")
            .WithTags("RuleEngine Admin");

        group.MapGet("/", async (ISender sender) =>
        {
            var result = await sender.Send(new GetRuleDefinitionsQuery());
            return Results.Ok(result);
        });

        group.MapPost("/", async (CreateRuleDefinitionCommand command, ISender sender) =>
        {
            var result = await sender.Send(command);
            return Results.Created($"/api/rule-definitions/{command.Code}", result);
        });

        group.MapPut("/{code}", async (string code, UpdateRuleDefinitionCommand command, ISender sender) =>
        {
            var updateCommand = command with { Code = code };
            await sender.Send(updateCommand);
            return Results.NoContent();
        });

        group.MapDelete("/{code}", async (string code, ISender sender) =>
        {
            await sender.Send(new DeleteRuleDefinitionCommand { Code = code });
            return Results.NoContent();
        });
    }
}
