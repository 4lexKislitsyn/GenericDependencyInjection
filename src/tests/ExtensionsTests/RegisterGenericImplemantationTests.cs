using GenericDependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ExtensionsTests
{
    public class RegisterGenericImplemantationTests
    {
        [Test]
        public void RegisterNullTypeTest()
        {
            var services = new ServiceCollection();
            services.AddGenericImplementations(null, ServiceLifetime.Scoped);
            Assert.AreEqual(0, services.Count);
        }
        [Test]
        public void RegisterOnlyPublicTest()
        {
            var services = new ServiceCollection();
            services.AddGenericImplementations(typeof(Interfaces.IGenericSample<>), options =>
            {
                options.ExcludeTypes.Add(Filters.OnlyPublic);
            });
            Assert.AreEqual(1, services.Count);
        }
    }
}
