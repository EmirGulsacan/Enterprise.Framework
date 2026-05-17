namespace Enterprise.Framework.Application.RuleEngine.Commands;

using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Domain.Rules;
using MediatR;

public sealed record CreateRuleDefinitionCommand : IRequest<long>
{
    public string Name { get; init; } = default!;
    public string Code { get; init; } = default!;
    public string Domain { get; init; } = default!;
    public string Description { get; init; } = string.Empty;
    public string WorkflowJson { get; init; } = default!;
}

class CreateRuleDefinitionCommandHandler : IRequestHandler<CreateRuleDefinitionCommand, long>
{
    private readonly IApplicationDbContext _context;

    public CreateRuleDefinitionCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<long> Handle(CreateRuleDefinitionCommand request, CancellationToken cancellationToken)
    {
        var entity = new RuleDefinition
        {
            Name = request.Name,
            Code = request.Code,
            Domain = request.Domain,
            Description = request.Description,
            WorkflowJson = request.WorkflowJson,
            IsActive = true
        };

        _context.GetDbSet<RuleDefinition>().Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
