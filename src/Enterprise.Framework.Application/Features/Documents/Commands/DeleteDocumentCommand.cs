namespace Enterprise.Framework.Application.Features.Documents.Commands;

using Enterprise.Framework.Application.Common.Exceptions;
using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Domain.Entities;
using MediatR;

public sealed record DeleteDocumentCommand(long Id) : IRequest<Unit>;

public class DeleteDocumentCommandHandler : IRequestHandler<DeleteDocumentCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public DeleteDocumentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(DeleteDocumentCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.GetDbSet<Document>().FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Document), request.Id);
        }

        _context.GetDbSet<Document>().Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
