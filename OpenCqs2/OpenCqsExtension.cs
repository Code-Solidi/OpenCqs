using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using OpenCqs2.Abstractions;

using System.Reflection;

namespace OpenCqs2
{
    public static class OpenCqsExtension
    {
        public static IServiceCollection AddHandlers(this IServiceCollection services, IEnumerable<Assembly> assemblies)
        {
            foreach (var assembly in assemblies)
            {
                services = services.AddHandlers(assembly);
            }

            return services;
        }

        public static IServiceCollection AddHandlers(this IServiceCollection services, Assembly assembly)
        {
            foreach (var type in OpenCqsExtension.GetHandlerTypes(assembly))
            {
                var service = (type as TypeInfo)?.ImplementedInterfaces.Single(x => x != typeof(IHandler) && typeof(IHandler).IsAssignableFrom(x));
                services.TryAddScoped(service!, provider =>
                {
                    var createProxy = type.GetMethod("CreateProxy", BindingFlags.Static | BindingFlags.NonPublic);
                    if (createProxy != null)
                    {
                        try
                        {
                            return createProxy.Invoke(null, new[] { provider })!;
                        }
                        catch
                        {
                            throw;
                        }
                    }

                    try
                    {
                        return Activator.CreateInstance(type)!;
                    }
                    catch (MissingMethodException)
                    {
                        return Activator.CreateInstance(type, new[] { provider })!;
                    }
                });
            }

            return services;

            //var factory = new DefaultServiceProviderFactory();
            //var serviceProvider = factory?.CreateServiceProvider(services); //services.BuildServiceProvider() -- difference??
            //using (var scope = serviceProvider?.CreateScope())
            //{
            //    services.TryAddScoped<IQueryHandler<DemoQuery, string?>>(provider =>
            //    {
            //        var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
            //        return LoggingProxy<DemoQueryHandler>.Create(new DemoQueryHandler(loggerFactory))!;
            //    });
            //    return services;
            //}
        }

        public static object? CreateProxy(this Type type, IServiceProvider provider)
        {
            var createProxy = type.GetMethod("CreateProxy");
            return createProxy?.Invoke(type, new[] { provider });
        }

        /// <summary>
        /// Gets the handler types from the assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>A list of Types.</returns>
        private static IEnumerable<Type> GetHandlerTypes(Assembly assembly)
        {
            _ = assembly ?? throw new ArgumentNullException(nameof(assembly));

            var allTypes = assembly.GetTypes().Where(x => (x as TypeInfo)?.ImplementedInterfaces.Count() != 0);
            var result = new List<Type>();
            foreach (var type in allTypes)
            {
                var implemented = (type as TypeInfo)?.ImplementedInterfaces;
                if (type.IsClass && (implemented?.Any(i => i == typeof(IHandler)) ?? false))
                {
                    result.Add(type);
                }
            }

            return result;
        }
    }
}