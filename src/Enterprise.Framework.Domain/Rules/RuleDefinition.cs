namespace Enterprise.Framework.Domain.Rules;

using Enterprise.Framework.Domain.Entities;

public class RuleDefinition : AuditableEntity
{
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string Domain { get; set; } = default!;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public string WorkflowJson { get; set; } = default!;
}
