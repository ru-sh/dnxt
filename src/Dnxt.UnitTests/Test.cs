using System;
using Dnxt.Parsing;

namespace Dnxt.UnitTests
{
    public interface IFoo
    {
        int A { get; }
        string B { get; }
        Type C { get; }
        IBar Bar { get; }
    }

    public interface IBar
    {
        string D { get; }
    }

    class Foo : IFoo
    {
        public Foo(int a, string b, Type c, IBar bar)
        {
            A = a;
            B = b;
            C = c;
            Bar = bar;
        }

        public int A { get; }
        public string B { get; }
        public Type C { get; }
        public IBar Bar { get; }
        public string X { get; }
    }

}