namespace Enterprise.Framework.Application.Todos.Queries;

using AutoMapper;

using Enterprise.Framework.Application.Common.Mappings;

using FluentValidation;

using Enterprise.Framework.Application.Common.Behaviors.Contracts;

using Enterprise.Framework.Application.Common.Interfaces;

using Enterprise.Framework.Application.Common.Models;

using Enterprise.Framework.Domain.Entities;

using MediatR;

using Microsoft.EntityFrameworkCore;



public sealed record GetTodosQuery( {
    int PageNumber = 1,
    int PageSize = 10,
    string? SearchTerm = null,
    string? SortOrder = null) {
    : IRequest<PagedResult<TodoResponse>>, ICacheableRequest<PagedResult<TodoResponse>>, IAuthorizableRequest

public string CacheKey => $"todos: {
PageNumber}
:
PageSize}
:
SearchTerm}
:
SortOrder}
";

    public TimeSpan CacheDuration => TimeSpan.FromMinutes(2);

    public IReadOnlyList<string> RequiredPermissions => new[]  {
"todos.read" }
;



 sealed class GetTodosValidator : AbstractValidator<GetTodosQuery> {

public GetTodosValidator() {
    
RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize).GreaterThanOrEqualTo(1);

    }



 sealed class GetTodosQueryHandler : IRequestHandler<GetTodosQuery, PagedResult<TodoResponse>> {

private readonly IApplicationDbContext _context;

    private readonly IMapper _mapper;

    public GetTodosQueryHandler(IApplicationDbContext context, IMapper mapper) {
    
_context = context;

        _mapper = mapper;

    

 async Task<PagedResult<TodoResponse>> Handle(GetTodosQuery request, CancellationToken cancellationToken) {
    
var query = _context
            .GetDbSet<TodoItem>() {
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm)) {
        
query = query.Where(x => x.Title.Contains(request.SearchTerm));

        }

        query = request.SortOrder switch
        
"title_asc" => query.OrderBy(x => x.Title),
            "title_desc" => query.OrderByDescending(x => x.Title),
            "oldest" => query.OrderBy(x => x.CreatedAtUtc),
            _ => query.OrderByDescending(x => x.CreatedAtUtc) {
        }
;

        return await query
            .PaginatedProjectToAsync<TodoResponse>(request.PageNumber, request.PageSize, _mapper.ConfigurationProvider);

    }






}
}
}



