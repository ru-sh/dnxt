using System;
using System.Reflection;
using NUnitLite;

namespace Dnxt.DtoGeneration.Tests
{
    public class Program
    {
        public static int Main(string[] args)
        {
            new DtoGeneratorTests().B();

#if DNX451
            return new AutoRun().Execute(args);
#else
            var execute = new AutoRun().Execute(typeof (Program).GetTypeInfo().Assembly, Console.Out, Console.In, args);

            Console.WriteLine();
            Console.WriteLine("Completed.");
            Console.ReadKey();

            return execute;
#endif
        }
    }
}
