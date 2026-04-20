namespace Enterprise.Framework.Application.Features.Employees.Queries;

using Enterprise.Framework.Application.Common.Interfaces;
using Enterprise.Framework.Application.Common.Models;
using Enterprise.Framework.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Enterprise.Framework.Application.Common.Mappings;

public sealed record GetEmployeesQuery : IRequest<PagedResult<EmployeeDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? SearchTerm { get; init; }
    public string? SortOrder { get; init; }
}

public class GetEmployeesQueryHandler : IRequestHandler<GetEmployeesQuery, PagedResult<EmployeeDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetEmployeesQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PagedResult<EmployeeDto>> Handle(GetEmployeesQuery request, CancellationToken cancellationToken)
    {
        var query = _context.GetDbSet<Employee>().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.ToLower();
            query = query.Where(x => 
                x.FirstName.ToLower().Contains(searchTerm) || 
                x.LastName.ToLower().Contains(searchTerm) || 
                x.Email.ToLower().Contains(searchTerm));
        }

        query = request.SortOrder switch
        {
            "firstName_desc" => query.OrderByDescending(x => x.FirstName),
            "firstName_asc" => query.OrderBy(x => x.FirstName),
            "lastName_desc" => query.OrderByDescending(x => x.LastName),
            "lastName_asc" => query.OrderBy(x => x.LastName),
            "id_asc" => query.OrderBy(x => x.Id),
            _ => query.OrderByDescending(x => x.Id)
        };

        return await query.PaginatedProjectToAsync<EmployeeDto>(request.PageNumber, request.PageSize, _mapper.ConfigurationProvider);
    }
}
