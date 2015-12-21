using System;

namespace Dnxt.UnitTests
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IFoo foo = new Foo(0, string.Empty, typeof(bool));
            
            var updated = foo
                .Set(f => f.A, 1)
                .Set(f => f.B, "test")
                .Apply();
            
            Console.WriteLine(foo.A);
        }
    }
}
