using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using SmartGrid.Application.Interfaces;

namespace SmartGrid.Application.Extensions
{
    internal static class MapperExtensions
    {
        public static IServiceCollection AddMappers(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            var mappers = assembly.GetTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .SelectMany(t => t.GetInterfaces(), (t, i) => new { Implementation = t, Interface = i })
                .Where(x => x.Interface.IsGenericType &&
                            x.Interface.GetGenericTypeDefinition() == typeof(IMapper<,>));

            foreach (var mapper in mappers)
            {
                services.AddTransient(mapper.Interface, mapper.Implementation);
                Console.WriteLine($"[INFO] Registered mapper: {mapper.Implementation.Name} as {mapper.Interface.Name}");
            }

            return services;
        }
    }
}
