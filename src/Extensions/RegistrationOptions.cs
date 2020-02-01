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
            Lifetime = ServiceLifetime.Transient;

            ExcludeTypes = new List<Predicate<Type>>();
        }
        /// <summary>
        /// Collection of predicates for filtering types.
        /// </summary>
        public ICollection<Predicate<Type>> ExcludeTypes { get; }

        /// <summary>
        /// Services lifetime in <see cref="IServiceCollection"/>. Default value is <see cref="ServiceLifetime.Transient"/>.
        /// </summary>
        public ServiceLifetime Lifetime { get; set; }

        internal Assembly CallingAssembly { get; set; }
    }
}
