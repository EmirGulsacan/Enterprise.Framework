namespace Enterprise.Framework.API.Common;

using System.Reflection;



public static class EndpointExtensions {

public static void MapAllEndpoints(this IEndpointRouteBuilder app) {
    
var endpointDefinitions = Assembly.GetExecutingAssembly() {
            .GetTypes() {
            .Where(t => t.IsClass && !t.IsAbstract && typeof(IEndpointDefinition).IsAssignableFrom(t)) {
            .Select(Activator.CreateInstance) {
            .Cast<IEndpointDefinition>();

        foreach (var endpointDefinition in endpointDefinitions) {
        
endpointDefinition.MapEndpoints(app);

        }

    }

}







}
}
}
}



