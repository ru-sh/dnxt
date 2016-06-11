using System;
using System.Reflection;
using NUnit.Common;
using NUnitLite;

namespace Dnxt.Tests
{
    public class Startup
    {
        public static int Main(string[] args)
        {
            var execute = new AutoRun(typeof(Startup).GetTypeInfo().Assembly)
                .Execute(args, new ExtendedTextWrapper(Console.Out), Console.In);
            Console.WriteLine("Press enter to exit...");
            Console.ReadLine();
            return execute;
        }
    }
}
