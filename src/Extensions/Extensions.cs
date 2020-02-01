using GenericDependencyInjection.Extensions;
using System;
using System.Linq;
using System.Reflection;

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

            var options = new RegistrationOptions
            {
                Lifetime = lifetime
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
            var options = new RegistrationOptions();
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

            bool FilterType(Type type) => !type.IsAbstract && !type.IsInterface
                    && type.GetInterfaces().Any(interfaceType => interfaceType.IsGenericType);

            Type[] genericInterfaceImplementations = null;

            switch (options.AssemblyToUse)
            {
                case RegistrationOptions.SearchIn.AllAssemblies:
                    var assemblies = AppDomain.CurrentDomain?.GetAssemblies()
                        .Where(assembly => !options.ExcludeAssemblies.Any(filter => filter(assembly)));
                    genericInterfaceImplementations = assemblies?.SelectMany(assembly => assembly.GetTypes().Where(FilterType))
                        .ToArray();
                    break;
                case RegistrationOptions.SearchIn.OnlyCallingAssembly:
                    genericInterfaceImplementations = Assembly.GetCallingAssembly().GetTypes().Where(FilterType).ToArray();
                    break;
            }

            if (genericInterfaceImplementations == null)
            {
                return;
            }

            foreach (var type in genericInterfaceImplementations)
            {
                var serviceTypes = type.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericType);

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

