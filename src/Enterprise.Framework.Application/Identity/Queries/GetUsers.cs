namespace Enterprise.Framework.Application.Identity.Queries;

using AutoMapper;
using Enterprise.Framework.Application.Common.Mappings;
using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Application.Common.Models;
using Enterprise.Framework.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

public record UserDto
{
    public long Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
}

public record GetUsersQuery : IRequest<PagedResult<UserDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchTerm { get; init; }
    public string? SortOrder { get; init; }
}

sealed class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PagedResult<UserDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetUsersQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PagedResult<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.GetDbSet<AppUser>().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(x => x.Email.Contains(request.SearchTerm) || x.FirstName.Contains(request.SearchTerm) || x.LastName.Contains(request.SearchTerm));
        }

        query = request.SortOrder switch
        {
            "email_desc" => query.OrderByDescending(x => x.Email),
            "email_asc" => query.OrderBy(x => x.Email),
            "id_asc" => query.OrderBy(x => x.Id),
            "id_desc" => query.OrderByDescending(x => x.Id),
            _ => query.OrderByDescending(x => x.Id)
        };

        return await query.PaginatedProjectToAsync<UserDto>(request.PageNumber, request.PageSize, _mapper.ConfigurationProvider);
    }
}
