/*
 * Copyright (c) 2021-2022 Code Solidi Ltd. All rights reserved.
 * Licensed under the OSL-3.0, https://opensource.org/licenses/OSL-3.0.
 */

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OpenCqs
{
    /// <summary>
    /// The extension class exposing the AddHandlers extension method.
    /// </summary>
    public static class OpenCqsExtension
    {
        /// <summary>
        /// Adds the handlers found in assembly to the service collection.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="assembly">The assembly.</param>
        /// <returns>An IServiceCollection.</returns>
        public static IServiceCollection AddHandlers(this IServiceCollection services, Assembly assembly)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            _ = assembly ?? throw new ArgumentNullException(nameof(assembly));

            var queryHandlerTypes = OpenCqsExtension.GetHandlerTypes<IQuery>(assembly);
            foreach (var handlerType in new[] { typeof(IQueryHandler<,>), typeof(IQueryHandlerAsync<,>) })
            {
                services.AddHandlersFor(handlerType, queryHandlerTypes);
            }

            var commandHandlerTypes = OpenCqsExtension.GetHandlerTypes<ICommand>(assembly);
            foreach (var handlerType in new[] { typeof(ICommandHandler<,>), typeof(ICommandHandlerAsync<,>) })
            {
                services.AddHandlersFor(handlerType, commandHandlerTypes);
            }

            return services;
        }

        /// <summary>
        /// Gets the handler types from the assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>A list of Types.</returns>
        private static IEnumerable<Type> GetHandlerTypes<T>(Assembly assembly)
        {
            _ = assembly ?? throw new ArgumentNullException(nameof(assembly));

            // decorating handlers are denoted by [Decorator] attribute, exclude them
            var allTypes = assembly.GetTypes().Where(x => x.GetCustomAttributes(typeof(DecoratorAttribute), false).Length == 0);

            return allTypes.Where(x =>
            {
                foreach (var item in (x as TypeInfo)?.ImplementedInterfaces ?? new Type[0])
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
            });
        }

        /// <summary>
        /// Adds the handlers for handlerType.
        /// </summary>
        /// <param name="services">The services.</param>
        /// <param name="handlerType">The handler type.</param>
        /// <param name="handlerTypes">The handler types.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S3011:Reflection should not be used to increase accessibility of classes, methods, or fields", Justification = "<Pending>")]
        private static void AddHandlersFor(this IServiceCollection services, Type handlerType, IEnumerable<Type> handlerTypes)
        {
            _ = services ?? throw new ArgumentNullException(nameof(services));
            _ = handlerTypes ?? throw new ArgumentNullException(nameof(handlerTypes));
            _ = handlerType ?? throw new ArgumentNullException(nameof(handlerType));

            foreach (var implementationType in handlerTypes)
            {
                try
                {
                    var genericInterface = (implementationType as TypeInfo).ImplementedInterfaces.Single(x => x.IsGenericType);
                    var typeArgs = genericInterface.IsGenericType && genericInterface.GetGenericTypeDefinition() == handlerType
                        ? genericInterface.GetGenericArguments()
                        : default;

                    if (typeArgs != default)
                    {
                        var serviceType = handlerType.MakeGenericType(typeArgs);
                        if (serviceType.IsAssignableFrom(implementationType))
                        {
                            services.TryAddScoped(serviceType, implementationType);
                            var service = services.SingleOrDefault(s => s.ServiceType == serviceType);

                            var instance = ActivatorUtilities.GetServiceOrCreateInstance(services.BuildServiceProvider(), service.ImplementationType);
                            var build = instance.GetType().GetMethod("Build", BindingFlags.Instance | BindingFlags.NonPublic);
                            instance = build.Invoke(instance, null);

                            // we have the instance, no need to create it multiple times, hence singleton.
                            var descriptor = ServiceDescriptor.Describe(serviceType, _ => instance, ServiceLifetime.Singleton);
                            if (descriptor != default)
                            {
                                services.Replace(descriptor);
                            }
                        }
                    }
                }
                catch (ArgumentException)
                {
                    // cannot create service type, this is OK
                }
            }
        }
    }
}