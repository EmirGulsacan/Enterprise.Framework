namespace Enterprise.Framework.Application.Common.Extensions;

using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text.Json;

public static class IQueryableExtensions
{
    public static IQueryable<T> ApplyGridOptions<T>(this IQueryable<T> query, string? sortOrder, string? filtersJson)
    {
        Dictionary<string, string>? filters = null;
        if (!string.IsNullOrWhiteSpace(filtersJson))
        {
            try
            {
                filters = JsonSerializer.Deserialize<Dictionary<string, string>>(filtersJson);
            }
            catch { }
        }

        if (filters != null && filters.Any())
        {
            foreach (var filter in filters)
            {
                var propertyName = filter.Key;
                var filterValue = filter.Value;
                if (!string.IsNullOrEmpty(filterValue))
                {
                    try
                    {
                        var parts = propertyName.Split('.');
                        var prop = string.Join(".", parts.Select(p => char.ToUpper(p[0]) + p.Substring(1)));
                        var type = typeof(T);
                        foreach (var part in parts)
                        {
                            var propInfo = type.GetProperty(char.ToUpper(part[0]) + part.Substring(1), System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                            if (propInfo != null)
                            {
                                type = propInfo.PropertyType;
                            }
                            else
                            {
                                type = null;
                                break;
                            }
                        }

                        if (type == typeof(string))
                        {
                            query = query.Where($"{prop} != null && {prop}.Contains(@0)", filterValue);
                        }
                        else if (type != null)
                        {
                            query = query.Where($"{prop}.ToString() == @0", filterValue);
                        }
                        else
                        {
                            query = query.Where($"{prop}.ToString().Contains(@0)", filterValue);
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(sortOrder))
        {
            try
            {
                var parts = sortOrder.Split('_');
                if (parts.Length == 2)
                {
                    var prop = char.ToUpper(parts[0][0]) + parts[0].Substring(1);
                    var dir = parts[1].ToLower() == "desc" ? "desc" : "asc";
                    query = query.OrderBy($"{prop} {dir}");
                }
            }
            catch
            {
            }
        }
        else
        {
            try
            {
                query = query.OrderBy("Id desc");
            }
            catch { }
        }

        return query;
    }
}
