namespace Enterprise.Framework.Domain.Rules;

using Enterprise.Framework.Domain.Common;
using Enterprise.Framework.Domain.Entities;

public class OperationRuleMapping : IEntity
{
    public long Id { get; set; }
    
    public long OperationId { get; set; }
    public OperationDefinition Operation { get; set; } = default!;

    public long RuleDefinitionId { get; set; }
    public RuleDefinition RuleDefinition { get; set; } = default!;
}
