namespace Enterprise.Framework.Application.Branches.Commands;

using Enterprise.Framework.Application.Common.Interfaces;

using Enterprise.Framework.Domain.Entities;

using Keycloak.Identity.Shared.Interfaces;

using MediatR;



public record CreateBranchCommand(string Name, string Code) : IRequest<long>;

public class CreateBranchCommandHandler : IRequestHandler<CreateBranchCommand, long> {

private readonly IApplicationDbContext _context;

    private readonly ICurrentUserService _currentUserService;

    public CreateBranchCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService) {
    
_context = context;

        _currentUserService = currentUserService;

    

 async Task<long> Handle(CreateBranchCommand request, CancellationToken cancellationToken) {
    
var entity = new Branch
        
Name = request.Name,
            Code = request.Code,
            OrganizationId = _currentUserService.OrganizationId
        }
;

        _context.GetDbSet<Branch>().Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;

    }
}



