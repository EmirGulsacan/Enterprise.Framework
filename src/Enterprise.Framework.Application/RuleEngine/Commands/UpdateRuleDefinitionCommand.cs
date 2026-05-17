namespace Enterprise.Framework.Application.RuleEngine.Commands;

using Enterprise.Framework.Application.Common.Exceptions;
using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Domain.Rules;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

public sealed record UpdateRuleDefinitionCommand : IRequest<Unit>
{
    public string Code { get; init; } = default!;
    public string Name { get; init; } = default!;
    public string Domain { get; init; } = default!;
    public string Description { get; init; } = string.Empty;
    public string WorkflowJson { get; init; } = default!;
    public bool IsActive { get; init; } = true;
}

class UpdateRuleDefinitionCommandHandler : IRequestHandler<UpdateRuleDefinitionCommand, Unit>
{
    private readonly IApplicationDbContext _context;
    private readonly IMemoryCache _cache;

    public UpdateRuleDefinitionCommandHandler(IApplicationDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<Unit> Handle(UpdateRuleDefinitionCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.GetDbSet<RuleDefinition>()
            .FirstOrDefaultAsync(r => r.Code == request.Code, cancellationToken);

        if (entity is null)
            throw new NotFoundException(nameof(RuleDefinition), request.Code);

        entity.Name = request.Name;
        entity.Domain = request.Domain;
        entity.Description = request.Description;
        entity.WorkflowJson = request.WorkflowJson;
        entity.IsActive = request.IsActive;

        await _context.SaveChangesAsync(cancellationToken);

        _cache.Remove($"rule_engine:{request.Code}");

        return Unit.Value;
    }
}
