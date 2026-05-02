namespace Enterprise.Framework.Application.Features.Employees.Rules;

using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Domain.Common;
using Enterprise.Framework.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

public class EmployeeEmailMustBeUniqueRule : IBusinessRule
{
    private readonly IApplicationDbContext _context;
    private readonly string _email;
    private readonly long? _excludeId;

    public EmployeeEmailMustBeUniqueRule(IApplicationDbContext context, string email, long? excludeId = null, int order = 1)
    {
        _context = context;
        _email = email;
        _excludeId = excludeId;
        Order = order;
    }

    public int Order { get; }

    public string ErrorCode => "EMPLOYEE_EMAIL_EXISTS";

    public string Message => $"'{_email}' e-posta adresi zaten başka bir çalışan tarafından kullanılmaktadır.";

    public async Task<bool> IsBrokenAsync(CancellationToken cancellationToken = default)
    {
        var query = _context.GetDbSet<Employee>().Where(e => e.Email == _email);
        
        if (_excludeId.HasValue)
        {
            query = query.Where(e => e.Id != _excludeId.Value);
        }

        var exists = await query.AnyAsync(cancellationToken);
        return exists;
    }
}
