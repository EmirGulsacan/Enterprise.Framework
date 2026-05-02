namespace Enterprise.Framework.Application.Common.Rules;

using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Domain.Common;
using System.Threading;
using System.Threading.Tasks;

public class EntityMustExistRule<T> : IBusinessRule where T : class, IEntity
{
    private readonly IApplicationDbContext _context;
    private readonly long _id;

    public EntityMustExistRule(IApplicationDbContext context, long id, int order = 1)
    {
        _context = context;
        _id = id;
        Order = order;
    }

    public int Order { get; }

    public string ErrorCode => "ENTITY_NOT_FOUND";

    public string Message => $"{typeof(T).Name} ile eşleşen #{_id} kimlikli kayıt bulunamadı.";

    public async Task<bool> IsBrokenAsync(CancellationToken cancellationToken = default)
    {
        var entity = await _context.GetDbSet<T>().FindAsync(new object[] { _id }, cancellationToken);
        return entity == null;
    }
}
