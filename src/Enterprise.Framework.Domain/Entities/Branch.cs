namespace Enterprise.Framework.Domain.Entities;

using Enterprise.Framework.Domain.Common;



public class Branch : BaseEntity, IOrganizationBoundEntity {

public string Name { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string? OrganizationId { get; set; }
}



