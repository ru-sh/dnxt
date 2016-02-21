using System;

namespace Dnxt.UnitTests
{
    public class IndexAttribute : Attribute
    {
        public bool IsUnique { get; set; }
    }
}