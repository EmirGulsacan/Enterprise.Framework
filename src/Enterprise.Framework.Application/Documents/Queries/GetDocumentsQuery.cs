namespace Enterprise.Framework.Application.Documents.Queries;

using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Application.Common.Models;
using Enterprise.Framework.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Enterprise.Framework.Application.Common.Mappings;

public record GetDocumentsQuery(int PageNumber = 1, int PageSize = 10) : IRequest<PagedResult<DocumentDto>>;

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
        var query = _context.Documents.AsNoTracking();
        return await query.PaginatedProjectToAsync<DocumentDto>(request.PageNumber, request.PageSize, _mapper.ConfigurationProvider);
    }
}
