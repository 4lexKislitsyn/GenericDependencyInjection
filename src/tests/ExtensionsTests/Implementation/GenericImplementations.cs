using ExtensionsTests.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExtensionsTests.Implementation
{
    public class PublicGenericImplementation : IGenericSample<object>
    {
    }

    internal class InternalGenericImplementations : IGenericSample<object>
    {

    }
}
