namespace Enterprise.Framework.Domain.Entities;

using Enterprise.Framework.Domain.Common;

public class Employee : AuditableEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;

    public ICollection<Asset> Assets { get; set; } = new List<Asset>();
    public ICollection<Labor> Labors { get; set; } = new List<Labor>();
}

