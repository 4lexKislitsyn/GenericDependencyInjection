using System;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class Extensions
    {
        /// <summary>
        /// Add all implementations of <paramref name="genericType"/> from all assemblies of current domain.
        /// </summary>
        /// <param name="services">Collection of service descriptors.</param>
        /// <param name="genericType">Generic type for implementations search.</param>
        /// <returns></returns>
        public static IServiceCollection AddSingletonGenericImplementations(this IServiceCollection services, Type genericType)
            => AddGenericImplementations(services, genericType, ServiceLifetime.Singleton);

        /// <summary>
        /// Add all implementations of <paramref name="genericType"/> from all assemblies of current domain.
        /// </summary>
        /// <param name="services">Collection of service descriptors.</param>
        /// <param name="genericType">Generic type for implementations search.</param>
        /// <returns></returns>
        public static IServiceCollection AddScopedGenericImplementations(this IServiceCollection services, Type genericType)
            => AddGenericImplementations(services, genericType, ServiceLifetime.Scoped);

        /// <summary>
        /// Add all implementations of <paramref name="genericType"/> from all assemblies of current domain.
        /// </summary>
        /// <param name="services">Collection of service descriptors.</param>
        /// <param name="genericType">Generic type for implementations search.</param>
        /// <returns></returns>
        public static IServiceCollection AddTransientGenericImplementations(this IServiceCollection services, Type genericType)
            => AddGenericImplementations(services, genericType, ServiceLifetime.Transient);

        /// <summary>
        /// Add all implementations of <paramref name="genericType"/> from all assemblies of current domain with slected lifetime.
        /// </summary>
        /// <param name="services">Collection of service descriptors.</param>
        /// <param name="genericType">Generic type for implementations search.</param>
        /// <returns></returns>
        public static IServiceCollection AddGenericImplementations(this IServiceCollection services, Type genericType, ServiceLifetime lifetime)
        {
            if (!(genericType?.IsGenericType ?? false))
            {
                return services;
            }

            bool FilterType(Type type) => !type.IsAbstract && !type.IsInterface
                    && type.GetInterfaces().Any(interfaceType => interfaceType.IsGenericType);

            var genericInterfaceImplementations = AppDomain.CurrentDomain?.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes().Where(FilterType))
                .ToArray();

            foreach (var type in genericInterfaceImplementations)
            {
                var serviceType = type.GetInterfaces()
                    .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericType);
                if (serviceType == null)
                {
                    continue;
                }
                services.Add(new ServiceDescriptor(serviceType, type, lifetime));
            }
            return services;
        }
    }
}

