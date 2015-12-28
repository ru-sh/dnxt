using System;
using Dnxt.Parsing;

namespace Dnxt.UnitTests
{
    public interface IFoo
    {
        int A { get; }
        string B { get; }
        Type C { get; }
    }

    class Foo : IFoo
    {
        public Foo(int a, string b, Type c)
        {
            A = a;
            B = b;
            C = c;
        }

        public int A { get; }
        public string B { get; }
        public Type C { get; }
        public string X { get; }
    }

}