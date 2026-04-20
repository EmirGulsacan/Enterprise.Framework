namespace Enterprise.Framework.Application.Features.Labors.Commands;

using Enterprise.Framework.Application.Common.Exceptions;
using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Domain.Entities;
using MediatR;

public sealed record DeleteLaborCommand(long Id) : IRequest<Unit>;

public class DeleteLaborCommandHandler : IRequestHandler<DeleteLaborCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public DeleteLaborCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteLaborCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.GetDbSet<Labor>().FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Labor), request.Id);
        }

        _context.GetDbSet<Labor>().Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
