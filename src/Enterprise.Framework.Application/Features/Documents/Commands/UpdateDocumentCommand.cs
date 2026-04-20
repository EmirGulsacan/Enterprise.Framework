namespace Enterprise.Framework.Application.Features.Documents.Commands;

using Enterprise.Framework.Application.Common.Exceptions;
using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

public sealed record UpdateDocumentCommand : IRequest<Unit>
{
    public long Id { get; init; }
    public string FileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public string Path { get; init; } = string.Empty;
    public string RelatedEntityType { get; init; } = string.Empty;
    public long RelatedEntityId { get; init; }
}

public class UpdateDocumentCommandHandler : IRequestHandler<UpdateDocumentCommand, Unit>
{
    private readonly IApplicationDbContext _context;

    public UpdateDocumentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(UpdateDocumentCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.GetDbSet<Document>().FindAsync(new object[] { request.Id }, cancellationToken);

        if (entity == null)
        {
            throw new NotFoundException(nameof(Document), request.Id);
        }

        entity.FileName = request.FileName;
        entity.ContentType = request.ContentType;
        entity.Path = request.Path;
        entity.RelatedEntityType = request.RelatedEntityType;
        entity.RelatedEntityId = request.RelatedEntityId;

        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
