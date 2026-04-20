namespace Enterprise.Framework.Application.Features.Assets.Commands;

using Enterprise.Framework.Application.Common.Exceptions;
using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Domain.Entities;
using MediatR;

public sealed record DeleteAssetCommand(long Id) : IRequest<Unit>;

public class DeleteAssetCommandHandler : IRequestHandler<DeleteAssetCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public DeleteAssetCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteAssetCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.GetDbSet<Asset>().FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Asset), request.Id);
        }

        _context.GetDbSet<Asset>().Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
