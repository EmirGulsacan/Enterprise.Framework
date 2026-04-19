namespace Enterprise.Framework.Application.Todos.Commands;

using AutoMapper;

using FluentValidation;

using Enterprise.Framework.Application.Common.Behaviors.Contracts;

using Enterprise.Framework.Application.Common.Exceptions;

using Enterprise.Framework.Application.Common.Interfaces;

using Enterprise.Framework.Domain.Entities;

using MediatR;

using Microsoft.EntityFrameworkCore;



public sealed record CompleteTodoCommand(long Id) : IRequest<TodoResponse>, IAuthorizableRequest {

public IReadOnlyList<string> RequiredPermissions => new[]  {
"todos.complete" }
;



 sealed class CompleteTodoValidator : AbstractValidator<CompleteTodoCommand> {

public CompleteTodoValidator() {
    
RuleFor(v => v.Id).GreaterThan(0);

    }



 sealed class CompleteTodoCommandHandler : IRequestHandler<CompleteTodoCommand, TodoResponse> {

private readonly IApplicationDbContext _context;

    private readonly IMapper _mapper;

    public CompleteTodoCommandHandler(IApplicationDbContext context, IMapper mapper) {
    
_context = context;

        _mapper = mapper;

    

 async Task<TodoResponse> Handle(CompleteTodoCommand request, CancellationToken cancellationToken) {
    
var todo = await _context.GetDbSet<TodoItem>() {
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (todo is null) {
        
throw new NotFoundException(nameof(TodoItem), request.Id);

        }

        if (!todo.IsCompleted) {
        
todo.IsCompleted = true;

            todo.CompletedAtUtc = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

        }

        return _mapper.Map<TodoResponse>(todo);

    }

}


}

}

}
}



