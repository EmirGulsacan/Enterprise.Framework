namespace Enterprise.Framework.Application.RuleEngine.Queries;

using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Domain.Rules;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed record OperationDefinitionDto(
    string Name, 
    string Code, 
    string Description, 
    bool IsActive,
    List<string> RuleCodes);

public sealed record GetOperationDefinitionsQuery : IRequest<List<OperationDefinitionDto>>;

class GetOperationDefinitionsQueryHandler : IRequestHandler<GetOperationDefinitionsQuery, List<OperationDefinitionDto>>
{
    private readonly IApplicationDbContext _context;

    public GetOperationDefinitionsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<OperationDefinitionDto>> Handle(GetOperationDefinitionsQuery request, CancellationToken cancellationToken)
    {
        var operations = await _context.GetDbSet<OperationDefinition>()
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .Include(x => x.RuleMappings)
                .ThenInclude(rm => rm.RuleDefinition)
            .ToListAsync(cancellationToken);

        return operations.Select(o => new OperationDefinitionDto(
            o.Name,
            o.Code,
            o.Description,
            o.IsActive,
            o.RuleMappings.Select(rm => rm.RuleDefinition.Code).ToList()
        )).ToList();
    }
}
