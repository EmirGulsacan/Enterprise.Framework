namespace Enterprise.Framework.Application.RuleEngine.Commands;

using Enterprise.Framework.Application.Common.Exceptions;
using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Domain.Rules;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed record DeleteOperationDefinitionCommand(string Code) : IRequest;

class DeleteOperationDefinitionCommandHandler : IRequestHandler<DeleteOperationDefinitionCommand>
{
    private readonly IApplicationDbContext _context;

    public DeleteOperationDefinitionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteOperationDefinitionCommand request, CancellationToken cancellationToken)
    {
        var operation = await _context.GetDbSet<OperationDefinition>()
            .FirstOrDefaultAsync(x => x.Code == request.Code, cancellationToken);

        if (operation is null)
            throw new NotFoundException(nameof(OperationDefinition), request.Code);

        operation.IsDeleted = true;
        operation.IsActive = false;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
