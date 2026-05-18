namespace Enterprise.Framework.Application.RuleEngine.Commands;

using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Domain.Rules;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed record CreateOperationDefinitionCommand(
    string Name, 
    string Code, 
    string Description, 
    List<string> RuleCodes) : IRequest<long>;

class CreateOperationDefinitionCommandHandler : IRequestHandler<CreateOperationDefinitionCommand, long>
{
    private readonly IApplicationDbContext _context;

    public CreateOperationDefinitionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<long> Handle(CreateOperationDefinitionCommand request, CancellationToken cancellationToken)
    {
        var operation = new OperationDefinition
        {
            Name = request.Name,
            Code = request.Code,
            Description = request.Description,
            IsActive = true
        };

        if (request.RuleCodes.Count > 0)
        {
            var rules = await _context.GetDbSet<RuleDefinition>()
                .Where(x => request.RuleCodes.Contains(x.Code))
                .ToListAsync(cancellationToken);

            foreach (var rule in rules)
            {
                operation.RuleMappings.Add(new OperationRuleMapping
                {
                    RuleDefinitionId = rule.Id,
                    Operation = operation
                });
            }
        }

        _context.GetDbSet<OperationDefinition>().Add(operation);
        await _context.SaveChangesAsync(cancellationToken);

        return operation.Id;
    }
}
