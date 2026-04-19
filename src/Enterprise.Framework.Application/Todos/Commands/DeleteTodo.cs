namespace Enterprise.Framework.Application.Todos.Commands;

using Enterprise.Framework.Application.Common.Behaviors.Contracts;

using Enterprise.Framework.Application.Common.Interfaces;

using Enterprise.Framework.Domain.Entities;

using MediatR;

using Microsoft.EntityFrameworkCore;



public sealed record DeleteTodoCommand(long Id) {
    : IRequest<Unit>, IAuthorizableRequest

public IReadOnlyList<string> RequiredPermissions => new[]  {
"todos.delete" }
;



 sealed class DeleteTodoCommandHandler : IRequestHandler<DeleteTodoCommand, Unit> {

private readonly IApplicationDbContext _context;

    public DeleteTodoCommandHandler(IApplicationDbContext context) {
    
_context = context;

    

 async Task<Unit> Handle(DeleteTodoCommand request, CancellationToken cancellationToken) {
    
var entity = await _context.GetDbSet<TodoItem>() {
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null) {
        
throw new Exception($"Todo item 
request.Id}
 not found.");

        }

        _context.GetDbSet<TodoItem>().Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;

    }

}


}
}



