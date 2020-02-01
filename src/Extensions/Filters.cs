using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenericDependencyInjection.Extensions
{
    public static class Filters
    {
        public static Predicate<Type> OnlyPublic { get; } = (type) => type.IsPublic;


        internal static Predicate<Type> ImplementsGenericInterface { get; } 
            = type => type.GetInterfaces().Any(interfaceType => interfaceType.IsGenericType);

        internal static Predicate<Type> NonInterface { get; } = type => !type.IsInterface;

        internal static Predicate<Type> NonAbstract { get; } = type => !type.IsAbstract;
    }
}
