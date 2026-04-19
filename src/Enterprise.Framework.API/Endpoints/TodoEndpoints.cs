namespace Enterprise.Framework.API.Endpoints;

using Enterprise.Framework.API.Common;

using Enterprise.Framework.Application.Common.Models;

using Enterprise.Framework.Application.Todos;

using Enterprise.Framework.Application.Todos.Commands;

using Enterprise.Framework.Application.Todos.Queries;

using MediatR;



public class TodoEndpoints : IEndpointDefinition {

public void MapEndpoints(IEndpointRouteBuilder app) {
    
var group = app.MapGroup("/api/todos") {
            .WithTags("Todos") {
            .RequireAuthorization();

        group.MapGet("/", async ([AsParameters] GetTodosQuery query, ISender sender, HttpContext ctx, CancellationToken ct) =>
        
var result = await sender.Send(query, ct);

            return Results.Ok(ApiResponse<PagedResult<TodoResponse>>.Ok(result, traceId: ctx.TraceIdentifier));

        }
);

        group.MapPost("/", async (CreateTodoRequest request, ISender sender, HttpContext ctx, CancellationToken ct) =>
        
var idempotencyKey = ctx.Request.Headers["X-Idempotency-Key"].FirstOrDefault();

            var key = string.IsNullOrWhiteSpace(idempotencyKey) ? Guid.NewGuid().ToString("N") : idempotencyKey;

            var result = await sender.Send(new CreateTodoCommand(request.Title, key), ct);

            return Results.Created($"/api/todos/
result.Id}
", ApiResponse<TodoResponse>.Ok(result, traceId: ctx.TraceIdentifier));

        




}
}



