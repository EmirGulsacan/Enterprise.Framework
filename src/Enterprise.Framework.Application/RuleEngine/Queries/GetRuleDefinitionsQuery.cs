namespace Enterprise.Framework.Application.RuleEngine.Queries;

using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Domain.Rules;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed record RuleDefinitionDto(string Name, string Code, string Domain, string Description, string WorkflowJson);

public sealed record GetRuleDefinitionsQuery : IRequest<List<RuleDefinitionDto>>;

class GetRuleDefinitionsQueryHandler : IRequestHandler<GetRuleDefinitionsQuery, List<RuleDefinitionDto>>
{
    private readonly IApplicationDbContext _context;

    public GetRuleDefinitionsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<RuleDefinitionDto>> Handle(GetRuleDefinitionsQuery request, CancellationToken cancellationToken)
    {
        return await _context.GetDbSet<RuleDefinition>()
            .AsNoTracking()
            .Where(r => r.IsActive)
            .Select(r => new RuleDefinitionDto(r.Name, r.Code, r.Domain, r.Description, r.WorkflowJson))
            .ToListAsync(cancellationToken);
    }
}
