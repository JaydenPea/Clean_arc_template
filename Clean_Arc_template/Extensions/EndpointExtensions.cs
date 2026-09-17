namespace CleanArc.Api.Extensions;

using System.Reflection;

public static class EndpointExtensions
{
    public static void MapApiEndpoints(this WebApplication app)
    {
        var endpointMappers = typeof(Program).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Static))
            .Where(m => m.Name.StartsWith("Map")
                && m.ReturnType == typeof(void)
                && m.GetParameters().Length == 1
                && m.GetParameters()[0].ParameterType == typeof(WebApplication))
            .ToList();

        foreach (var method in endpointMappers)
        {
            method.Invoke(null, [app]);
        }
    }
}
