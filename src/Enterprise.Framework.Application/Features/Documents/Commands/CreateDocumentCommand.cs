namespace Enterprise.Framework.Application.Features.Documents.Commands;

using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Domain.Entities;
using MediatR;

public sealed record CreateDocumentCommand : IRequest<long>
{
    public string FileName { get; init; } = default!;
    public string ContentType { get; init; } = default!;
    public string Path { get; init; } = default!;
    public string RelatedEntityType { get; init; } = default!;
    public long RelatedEntityId { get; init; }
}

public class CreateDocumentCommandHandler : IRequestHandler<CreateDocumentCommand, long>
{
    private readonly IApplicationDbContext _context;

    public CreateDocumentCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<long> Handle(CreateDocumentCommand request, CancellationToken cancellationToken)
    {
        var entity = new Document
        {
            FileName = request.FileName,
            ContentType = request.ContentType,
            Path = request.Path,
            RelatedEntityType = request.RelatedEntityType,
            RelatedEntityId = request.RelatedEntityId
        };

        _context.GetDbSet<Document>().Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
