namespace Enterprise.Framework.Application.Branches.Queries;

using AutoMapper;

using Enterprise.Framework.Application.Common.Interfaces;

using Enterprise.Framework.Application.Common.Mappings;

using Enterprise.Framework.Application.Common.Models;

using Enterprise.Framework.Domain.Entities;

using MediatR;

using Microsoft.EntityFrameworkCore;



public record GetBranchesQuery(int PageNumber = 1, int PageSize = 10, string? SearchTerm = null, string? SortOrder = null) {
    : IRequest<PagedResult<BranchDto>>;

public record BranchDto(long Id, string Name, string Code, string? OrganizationId) : IMapFrom<Branch>;

public class GetBranchesQueryHandler : IRequestHandler<GetBranchesQuery, PagedResult<BranchDto>> {

private readonly IApplicationDbContext _context;

    private readonly IMapper _mapper;

    public GetBranchesQueryHandler(IApplicationDbContext context, IMapper mapper) {
    
_context = context;

        _mapper = mapper;

    

 async Task<PagedResult<BranchDto>> Handle(GetBranchesQuery request, CancellationToken cancellationToken) {
    
var query = _context.GetDbSet<Branch>() {
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm)) {
        
query = query.Where(x => x.Name.Contains(request.SearchTerm) || x.Code.Contains(request.SearchTerm));

        }

        query = request.SortOrder switch
        
"name_desc" => query.OrderByDescending(x => x.Name),
            "name_asc" => query.OrderBy(x => x.Name),
            "code_desc" => query.OrderByDescending(x => x.Code),
            "code_asc" => query.OrderBy(x => x.Code),
            "id_asc" => query.OrderBy(x => x.Id),
            "id_desc" => query.OrderByDescending(x => x.Id),
            _ => query.OrderByDescending(x => x.Id) {
        }
;

        return await query
            .PaginatedProjectToAsync<BranchDto>(request.PageNumber, request.PageSize, _mapper.ConfigurationProvider);

    }

}


}






}
}



