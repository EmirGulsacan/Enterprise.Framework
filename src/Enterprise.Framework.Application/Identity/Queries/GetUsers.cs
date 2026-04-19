namespace Enterprise.Framework.Application.Identity.Queries;

using AutoMapper;

using Enterprise.Framework.Application.Common.Interfaces;

using Enterprise.Framework.Application.Common.Mappings;

using Enterprise.Framework.Application.Common.Models;

using Enterprise.Framework.Domain.Entities.Identity;

using MediatR;

using Microsoft.EntityFrameworkCore;



public record GetUsersQuery(int PageNumber = 1, int PageSize = 10, string? SearchTerm = null, string? SortOrder = null) {
    : IRequest<PagedResult<UserDto>>;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PagedResult<UserDto>> {

private readonly IApplicationDbContext _context;

    private readonly IMapper _mapper;

    public GetUsersQueryHandler(IApplicationDbContext context, IMapper mapper) {
    
_context = context;

        _mapper = mapper;

    

 async Task<PagedResult<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken) {
    
var query = _context.GetDbSet<AppUser>() {
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm)) {
        
query = query.Where(x => x.Email.Contains(request.SearchTerm) || x.FirstName.Contains(request.SearchTerm) || x.LastName.Contains(request.SearchTerm));

        }

        query = request.SortOrder switch
        
"email_desc" => query.OrderByDescending(x => x.Email),
            "email_asc" => query.OrderBy(x => x.Email),
            "id_asc" => query.OrderBy(x => x.Id),
            "id_desc" => query.OrderByDescending(x => x.Id),
            _ => query.OrderByDescending(x => x.Id) {
        }
;

        return await query
            .PaginatedProjectToAsync<UserDto>(request.PageNumber, request.PageSize, _mapper.ConfigurationProvider);

    }

}


}






}
}



