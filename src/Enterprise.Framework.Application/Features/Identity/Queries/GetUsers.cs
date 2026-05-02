namespace Enterprise.Framework.Application.Features.Identity.Queries;

using AutoMapper;
using Enterprise.Framework.Application.Common.Mappings;
using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Application.Common.Models;
using Enterprise.Framework.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Enterprise.Framework.Application.Features.Identity;
using Enterprise.Framework.Application.Common.Extensions;

public sealed record GetUsersQuery : IRequest<PagedResult<UserDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchTerm { get; init; }
    public string? SortOrder { get; init; }
    public string? FiltersJson { get; init; }
}

sealed class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PagedResult<UserDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;

    public GetUsersQueryHandler(IApplicationDbContext context, IMapper mapper, IConfiguration configuration)
    {
        _context = context;
        _mapper = mapper;
        _configuration = configuration;
    }

    public async Task<PagedResult<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var query = _context.GetDbSet<AppUser>().AsNoTracking();

        var bootstrapAdminEmail = _configuration["Security:BootstrapAdminEmail"];

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            query = query.Where(x => x.Email.Contains(request.SearchTerm) || x.FirstName.Contains(request.SearchTerm) || x.LastName.Contains(request.SearchTerm));
        }

        query = query.ApplyGridOptions(request.SortOrder, request.FiltersJson);

        var result = await query.PaginatedProjectToAsync<UserDto>(request.PageNumber, request.PageSize, _mapper.ConfigurationProvider);
        if (!string.IsNullOrEmpty(bootstrapAdminEmail))
        {
            foreach (var user in result.Items)
            {
                user.IsSystemAdmin = string.Equals(user.Email, bootstrapAdminEmail, StringComparison.OrdinalIgnoreCase);
            }
        }
        return result;
    }
}


