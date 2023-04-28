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
            var types = GetHandlerTypes(assembly);
            foreach (var type in types)
            {
                var service = (type as TypeInfo)?.ImplementedInterfaces.Single(x => x != typeof(IHandler) && typeof(IHandler).IsAssignableFrom(x));
                services.TryAddScoped(service!, provider =>
                {
                    var createProxy = type.GetMethod("CreateProxy", BindingFlags.Static | BindingFlags.NonPublic);
                    if (createProxy == null)
                    {
                        throw new InvalidOperationException($"Type {type.FullName} does not define method CreateProxy.");
                    }

                    try
                    {
                        //return createProxy.Invoke(type, new[] { provider })!;
                        return createProxy.Invoke(null, new[] { provider })!;
                    }
                    catch
                    {
                        throw;
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

            var interfaces = new[] { typeof(IQueryHandler<,>), typeof(ICommandHandler<,>) };

            //var allTypes = assembly.GetTypes().Where(x => (x as TypeInfo)?.ImplementedInterfaces.Count(i => interfaces.Any(n => n == i)) != 0);
            var allTypes = assembly.GetTypes().Where(x => (x as TypeInfo)?.ImplementedInterfaces.Count() != 0);
            var result = new List<Type>();
            foreach (var type in allTypes)
            {
                //var x = (type as TypeInfo)?.ImplementedInterfaces.Any(x => interfaces.All(i => i.IsSubclassOf(x)));
                var implemented = (type as TypeInfo)?.ImplementedInterfaces;
                /*var x = service?.Any(x => interfaces.All(i => x.IsSubclassOf(i)));
                var y = service?.Where(i => i.GetType() == typeof(IQueryHandler<,>) || i.GetType().IsSubclassOf(typeof(IQueryHandler<,>)));
                var z = service?.Where(i => i.GetType() == typeof(IHandler) || i.GetType().IsSubclassOf(typeof(IHandler)));*/
                if (type.IsClass && (implemented?.Any(i => i == typeof(IHandler)) ?? false))
                {
                    result.Add(type);
                }
            }

            return result;
            /*foreach (var type in allTypes)
            {
                //var decoratedAttributes = type.GetCustomAttributes(typeof(DecorateAttribute), false);
                var attributes = type.GetCustomAttributes(typeof(Attribute), true)
                    .Cast<Attribute>()
                    .Where(a => a.GetType() == typeof(DecorateAttribute) || a.GetType().IsSubclassOf(typeof(DecorateAttribute)));

                foreach (var attribute in attributes.Cast<DecorateAttribute>())
                {
                    Console.WriteLine($"Name: {attribute.GetType().Name}, Decorates: {attribute.Type}");
                }
            }*/

            //return allTypes;

            /*return allTypes.Where(x =>
            {
                foreach (var item in (x as TypeInfo)?.ImplementedInterfaces ?? Array.Empty<Type>())
                {
                    var typeArgs = item.GetGenericArguments();
                    if (typeArgs.Length == 2)
                    {
                        var itemKind = ((TypeInfo)typeArgs[0]).ImplementedInterfaces.FirstOrDefault();
                        if (itemKind == typeof(T))
                        {
                            return true;
                        }
                    }
                }

                return false;
            });*/
        }


    }
}