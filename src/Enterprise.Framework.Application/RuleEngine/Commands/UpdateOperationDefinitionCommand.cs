namespace Enterprise.Framework.Application.RuleEngine.Commands;

using Enterprise.Framework.Application.Common.Exceptions;
using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Domain.Rules;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed record UpdateOperationDefinitionCommand(
    string Code,
    string Name, 
    string Description, 
    bool IsActive,
    List<string> RuleCodes) : IRequest;

class UpdateOperationDefinitionCommandHandler : IRequestHandler<UpdateOperationDefinitionCommand>
{
    private readonly IApplicationDbContext _context;

    public UpdateOperationDefinitionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateOperationDefinitionCommand request, CancellationToken cancellationToken)
    {
        var operation = await _context.GetDbSet<OperationDefinition>()
            .Include(x => x.RuleMappings)
            .FirstOrDefaultAsync(x => x.Code == request.Code, cancellationToken);

        if (operation is null)
            throw new NotFoundException(nameof(OperationDefinition), request.Code);

        operation.Name = request.Name;
        operation.Description = request.Description;
        operation.IsActive = request.IsActive;

        // Clear existing mappings
        _context.GetDbSet<OperationRuleMapping>().RemoveRange(operation.RuleMappings);
        operation.RuleMappings.Clear();

        if (request.RuleCodes.Count > 0)
        {
            var rules = await _context.GetDbSet<RuleDefinition>()
                .Where(x => request.RuleCodes.Contains(x.Code))
                .ToListAsync(cancellationToken);

            foreach (var rule in rules)
            {
                operation.RuleMappings.Add(new OperationRuleMapping
                {
                    OperationId = operation.Id,
                    RuleDefinitionId = rule.Id
                });
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        
        return Unit.Value;
    }
}
