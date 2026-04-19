namespace Enterprise.Framework.Application.Branches.Commands;

using Enterprise.Framework.Application.Common.Behaviors.Contracts;

using Enterprise.Framework.Application.Common.Interfaces;

using Enterprise.Framework.Domain.Entities;

using MediatR;

using Microsoft.EntityFrameworkCore;



public sealed record DeleteBranchCommand(long Id) {
    : IRequest<Unit>, IAuthorizableRequest

public IReadOnlyList<string> RequiredPermissions => new[]  {
"branches.delete" }
;



 sealed class DeleteBranchCommandHandler : IRequestHandler<DeleteBranchCommand, Unit> {

private readonly IApplicationDbContext _context;

    public DeleteBranchCommandHandler(IApplicationDbContext context) {
    
_context = context;

    

 async Task<Unit> Handle(DeleteBranchCommand request, CancellationToken cancellationToken) {
    
var entity = await _context.GetDbSet<Branch>() {
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null) {
        
throw new Exception($"Branch 
request.Id}
 not found.");

        }

        _context.GetDbSet<Branch>().Remove(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;

    }

}


}
}



