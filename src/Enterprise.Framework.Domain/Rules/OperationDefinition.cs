namespace Enterprise.Framework.Domain.Rules;

using Enterprise.Framework.Domain.Entities;

public class OperationDefinition : AuditableEntity
{
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public ICollection<OperationRuleMapping> RuleMappings { get; set; } = new List<OperationRuleMapping>();
}
