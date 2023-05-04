using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using OpenCqs2.Abstractions;

using System.Reflection;

namespace OpenCqs2
{
    /// <summary>
    ///   <br />
    /// </summary>
    /// TODO Edit XML Comment Template for OpenCqsExtension
    public static class OpenCqsExtension
    {
        /// <summary>Adds the handlers.</summary>
        /// <param name="services">The services.</param>
        /// <param name="assemblies">The assemblies.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        /// <exception cref="System.ArgumentNullException">services
        /// or
        /// assemblies</exception>
        /// TODO Edit XML Comment Template for AddHandlers
        public static IServiceCollection AddHandlers(this IServiceCollection services, IEnumerable<Assembly> assemblies)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            _ = assemblies ?? throw new ArgumentNullException(nameof(assemblies));

            foreach (var assembly in assemblies)
            {
                services = services.AddHandlers(assembly);
            }

            return services;
        }

        /// <summary>Adds the handlers.</summary>
        /// <param name="services">The services.</param>
        /// <param name="assembly">The assembly.</param>
        /// <returns>
        ///   <br />
        /// </returns>
        /// <exception cref="System.ArgumentNullException">services
        /// or
        /// assembly</exception>
        /// TODO Edit XML Comment Template for AddHandlers
        public static IServiceCollection AddHandlers(this IServiceCollection services, Assembly assembly)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            _ = assembly ?? throw new ArgumentNullException(nameof(assembly));

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