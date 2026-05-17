namespace Enterprise.Framework.Application.RuleEngine.Commands;

using Enterprise.Framework.Application.Common.Exceptions;
using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Domain.Rules;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

public sealed record DeleteRuleDefinitionCommand : IRequest<Unit>
{
    public string Code { get; init; } = default!;
}

class DeleteRuleDefinitionCommandHandler : IRequestHandler<DeleteRuleDefinitionCommand, Unit>
{
    private readonly IApplicationDbContext _context;
    private readonly IMemoryCache _cache;

    public DeleteRuleDefinitionCommandHandler(IApplicationDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<Unit> Handle(DeleteRuleDefinitionCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.GetDbSet<RuleDefinition>()
            .FirstOrDefaultAsync(r => r.Code == request.Code, cancellationToken);

        if (entity is null)
            throw new NotFoundException(nameof(RuleDefinition), request.Code);

        entity.IsActive = false;

        await _context.SaveChangesAsync(cancellationToken);

        _cache.Remove($"rule_engine:{request.Code}");

        return Unit.Value;
    }
}
