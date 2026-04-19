namespace Enterprise.Framework.Application.Common.Mappings;

using AutoMapper;
using AutoMapper.QueryableExtensions;
using Enterprise.Framework.Application.Common.Models;
using Microsoft.EntityFrameworkCore;

public static class MappingExtensions
{
    public static Task<PagedResult<TDestination>> PaginatedListAsync<TDestination>(
        this IQueryable<TDestination> queryable, int pageNumber, int pageSize)
        where TDestination : class
        => PagedResultExtensions.CreateAsync(queryable.AsNoTracking(), pageNumber, pageSize);

    public static Task<PagedResult<TDestination>> PaginatedProjectToAsync<TDestination>(
        this IQueryable queryable, int pageNumber, int pageSize, IConfigurationProvider configuration)
        where TDestination : class
        => PagedResultExtensions.CreateAsync(
            queryable.ProjectTo<TDestination>(configuration).AsNoTracking(),
            pageNumber,
            pageSize);

    public static Task<List<TDestination>> ProjectToListAsync<TDestination>(
        this IQueryable queryable, IConfigurationProvider configuration, CancellationToken ct = default)
        where TDestination : class
        => queryable.ProjectTo<TDestination>(configuration).AsNoTracking().ToListAsync(ct);
}

static class PagedResultExtensions
{
    public static async Task<PagedResult<T>> CreateAsync<T>(
        IQueryable<T> source, int pageNumber, int pageSize)
    {
        var count = await source.CountAsync();
        var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PagedResult<T>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = count,
            TotalPages = (int)Math.Ceiling(count / (double)pageSize)
        };
    }
}
