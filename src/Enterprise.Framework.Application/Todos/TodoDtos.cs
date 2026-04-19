namespace Enterprise.Framework.Application.Todos;

using Enterprise.Framework.Application.Common.Mappings;

using Enterprise.Framework.Domain.Entities;



public record TodoResponse(long Id, string Title, bool IsCompleted, DateTime CreatedAtUtc, DateTime? CompletedAtUtc) : IMapFrom<TodoItem>;
}



