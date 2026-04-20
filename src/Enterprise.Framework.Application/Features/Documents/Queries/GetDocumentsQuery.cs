namespace Enterprise.Framework.Application.Features.Documents.Queries;

using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Application.Common.Models;
using Enterprise.Framework.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Enterprise.Framework.Application.Common.Mappings;

public sealed record GetDocumentsQuery : IRequest<PagedResult<DocumentDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchTerm { get; init; }
    public string? SortOrder { get; init; }
}

public class GetDocumentsQueryHandler : IRequestHandler<GetDocumentsQuery, PagedResult<DocumentDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetDocumentsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PagedResult<DocumentDto>> Handle(GetDocumentsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.GetDbSet<Document>().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(x => x.FileName.ToLower().Contains(searchTerm));
        }

        query = request.SortOrder switch
        {
            "fileName_desc" => query.OrderByDescending(x => x.FileName),
            "fileName_asc" => query.OrderBy(x => x.FileName),
            "id_asc" => query.OrderBy(x => x.Id),
            _ => query.OrderByDescending(x => x.Id)
        };

        return await query.PaginatedProjectToAsync<DocumentDto>(request.PageNumber, request.PageSize, _mapper.ConfigurationProvider);
    }
}
