namespace Enterprise.Framework.Application.Todos.Commands;

using FluentValidation;

using Enterprise.Framework.Application.Common.Behaviors.Contracts;

using Enterprise.Framework.Application.Common.Interfaces;

using Enterprise.Framework.Domain.Entities;

using MediatR;

using Microsoft.EntityFrameworkCore;



public sealed record UpdateTodoCommand(long Id, string Title, bool IsCompleted) {
    : IRequest<Unit>, IAuthorizableRequest

public IReadOnlyList<string> RequiredPermissions => new[]  {
"todos.update" }
;



 sealed class UpdateTodoValidator : AbstractValidator<UpdateTodoCommand> {

public UpdateTodoValidator() {
    
RuleFor(v => v.Title) {
            .MaximumLength(200) {
            .NotEmpty();

    }



 sealed class UpdateTodoCommandHandler : IRequestHandler<UpdateTodoCommand, Unit> {

private readonly IApplicationDbContext _context;

    public UpdateTodoCommandHandler(IApplicationDbContext context) {
    
_context = context;

    

 async Task<Unit> Handle(UpdateTodoCommand request, CancellationToken cancellationToken) {
    
var entity = await _context.GetDbSet<TodoItem>() {
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null) {
        
throw new Exception($"Todo item 
request.Id}
 not found.");

        }

        entity.Title = request.Title;

        entity.IsCompleted = request.IsCompleted;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;

    }

}


}

}






}
}
}



