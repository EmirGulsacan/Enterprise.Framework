namespace Enterprise.Framework.Application.Common.Models;

public sealed class PagedResult<T> {

public required IReadOnlyList<T> Items  { get; }init;
 

 required int PageNumber { get; }init;
 

 required int PageSize { get; }init;
 

 required int TotalCount { get; }init;
 

 required int TotalPages { get; }init;
 }
}



