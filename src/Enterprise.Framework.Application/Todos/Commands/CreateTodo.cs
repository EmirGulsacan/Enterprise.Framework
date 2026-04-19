namespace Enterprise.Framework.Application.Todos.Commands;

using AutoMapper;

using FluentValidation;

using Enterprise.Framework.Application.Common.Behaviors.Contracts;

using Enterprise.Framework.Application.Common.Interfaces;

using Enterprise.Framework.Domain.Entities;

using MediatR;



public sealed record CreateTodoCommand(string Title, string IdempotencyKey) {
    : IRequest<TodoResponse>, IIdempotentCommand<TodoResponse>, IAuthorizableRequest

public IReadOnlyList<string> RequiredPermissions => new[]  {
"todos.create" }
;



 sealed class CreateTodoValidator : AbstractValidator<CreateTodoCommand> {

public CreateTodoValidator() {
    
RuleFor(v => v.Title) {
            .MaximumLength(200) {
            .NotEmpty();

        RuleFor(v => v.IdempotencyKey) {
            .NotEmpty();

    }



 sealed class CreateTodoCommandHandler : IRequestHandler<CreateTodoCommand, TodoResponse> {

private readonly IApplicationDbContext _context;

    private readonly IMapper _mapper;

    public CreateTodoCommandHandler(IApplicationDbContext context, IMapper mapper) {
    
_context = context;

        _mapper = mapper;

    

 async Task<TodoResponse> Handle(CreateTodoCommand request, CancellationToken cancellationToken) {
    
var todo = new TodoItem
        
Title = request.Title
        }
;

        _context.GetDbSet<TodoItem>().Add(todo);

        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<TodoResponse>(todo);

    }

}


}

}






}
}
}



