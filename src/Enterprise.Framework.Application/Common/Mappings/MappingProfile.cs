namespace Enterprise.Framework.Application.Common.Mappings;

using AutoMapper;
using System.Reflection;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        ApplyMappingsFromAssembly(Assembly.GetExecutingAssembly());
    }

    private void ApplyMappingsFromAssembly(Assembly assembly)
    {
        var types = assembly.GetExportedTypes()
            .Where(t => t.GetInterfaces().Any(i =>
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapFrom<>)))
            .ToList();

        foreach (var type in types)
        {
            var interfaceType = type.GetInterface("IMapFrom`1");
            var methodInfo = type.GetMethod("Mapping") ?? interfaceType?.GetMethod("Mapping");
            var hasDefaultConstructor = type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null) != null;

            if (hasDefaultConstructor)
            {
                var instance = Activator.CreateInstance(type);
                methodInfo?.Invoke(instance, new object[] { this });
            }
            else
            {
                var sourceType = interfaceType?.GetGenericArguments()[0];
                if (sourceType != null)
                {
                    CreateMap(sourceType, type);
                }
            }
        }
    }
}
