using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace GenericDependencyInjection.Extensions
{
    public static class Extensions
    {
        /// <summary>
        /// Add all implementations of <paramref name="genericType"/> from all assemblies of current domain with slected lifetime.
        /// </summary>
        /// <param name="services">Collection of service descriptors.</param>
        /// <param name="genericType">Generic type for implementations search.</param>
        /// <returns></returns>
        public static IServiceCollection AddGenericImplementations(this IServiceCollection services, Type genericType, ServiceLifetime lifetime)
        {

            var options = new RegistrationOptions
            {
                Lifetime = lifetime,
                CallingAssembly = Assembly.GetCallingAssembly(),
            };

            RegisterByOptions(services, genericType, options);

            return services;
        }
        /// <summary>
        /// Add all implementations of <paramref name="genericType"/> using <see cref="RegistrationOptions"/>.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="genericType"></param>
        /// <param name="configure">Configure the options of search.</param>
        /// <returns></returns>
        public static IServiceCollection AddGenericImplementations(this IServiceCollection services, Type genericType, Action<RegistrationOptions> configure)
        {
            var options = new RegistrationOptions()
            {
                CallingAssembly = Assembly.GetCallingAssembly()
            };
            configure?.Invoke(options);

            RegisterByOptions(services, genericType, options);

            return services;
        }

        private static void RegisterByOptions(IServiceCollection services, Type genericType, RegistrationOptions options)
        {

            if (!(genericType?.IsGenericType ?? false))
            {
                return;
            }

            var types = options.CallingAssembly.GetTypes();

            bool FilterType(Type type) => Filters.NonAbstract(type) 
                && Filters.NonInterface(type)
                && Filters.ImplementsGenericInterface(type);

            bool ImplementsGenericType(Type type) 
                => type.IsGenericType && type.GetGenericTypeDefinition() == genericType;

            var filteredTypes = types.Where(FilterType).ToArray();

            foreach (var type in filteredTypes)
            {
                var serviceTypes = type.GetInterfaces().Where(ImplementsGenericType);

                if (!serviceTypes.Any() || options.ExcludeTypes.Any(filter => filter(type)))
                {
                    continue;
                }

                foreach (var serviceType in serviceTypes)
                {
                    services.Add(new ServiceDescriptor(serviceType, type, options.Lifetime));
                }
            }
        }
    }
}

