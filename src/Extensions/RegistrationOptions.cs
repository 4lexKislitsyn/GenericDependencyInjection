using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace GenericDependencyInjection.Extensions
{
    public class RegistrationOptions
    {
        internal RegistrationOptions()
        {
            AssemblyToUse = SearchIn.AllAssemblies;
            Lifetime = ServiceLifetime.Transient;

            ExcludeTypes = new List<Predicate<Type>>();
            ExcludeAssemblies = new List<Predicate<Assembly>>();
        }
        /// <summary>
        /// Collection of predicates for filtering types.
        /// </summary>
        public ICollection<Predicate<Type>> ExcludeTypes { get; }

        /// <summary>
        /// Collection of predicates for filtering assemblies.
        /// </summary>
        public ICollection<Predicate<Assembly>> ExcludeAssemblies { get; }

        /// <summary>
        /// Assembly/assemblies for Types serach. Default value is <see cref="SearchIn.AllAssemblies"/>.
        /// </summary>
        public SearchIn AssemblyToUse { get; set; }

        /// <summary>
        /// Services lifetime in <see cref="IServiceCollection"/>. Default value is <see cref="ServiceLifetime.Transient"/>.
        /// </summary>
        public ServiceLifetime Lifetime { get; set; }

        /// <summary>
        /// Specifies where looking for types.
        /// </summary>
        public enum SearchIn
        {
            AllAssemblies,
            OnlyCallingAssembly
        }
    }
}
