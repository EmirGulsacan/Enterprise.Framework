namespace Enterprise.Framework.Application.Branches.Commands;

using FluentValidation;

using Enterprise.Framework.Application.Common.Behaviors.Contracts;

using Enterprise.Framework.Application.Common.Interfaces;

using Enterprise.Framework.Domain.Entities;

using MediatR;

using Microsoft.EntityFrameworkCore;



public sealed record UpdateBranchCommand(long Id, string Name, string Code) {
    : IRequest<Unit>, IAuthorizableRequest

public IReadOnlyList<string> RequiredPermissions => new[]  {
"branches.update" }
;



 sealed class UpdateBranchValidator : AbstractValidator<UpdateBranchCommand> {

public UpdateBranchValidator() {
    
RuleFor(v => v.Name).NotEmpty().MaximumLength(200);

        RuleFor(v => v.Code).NotEmpty().MaximumLength(50);

    }



 sealed class UpdateBranchCommandHandler : IRequestHandler<UpdateBranchCommand, Unit> {

private readonly IApplicationDbContext _context;

    public UpdateBranchCommandHandler(IApplicationDbContext context) {
    
_context = context;

    

 async Task<Unit> Handle(UpdateBranchCommand request, CancellationToken cancellationToken) {
    
var entity = await _context.GetDbSet<Branch>() {
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        if (entity == null) {
        
throw new Exception($"Branch 
request.Id}
 not found.");

        }

        entity.Name = request.Name;

        entity.Code = request.Code;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;

    }

}


}

}
}



