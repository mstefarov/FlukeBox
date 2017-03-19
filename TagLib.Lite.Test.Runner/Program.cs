using System;
using System.Reflection;
using NUnit.Common;
using NUnitLite;
using TagLib.Tests;

namespace TagLib.Lite.Test.Runner
{
    class Program
    {
        public static int Main(string[] args)
        {
            int result = new AutoRun(typeof(MySetUpClass).GetTypeInfo().Assembly)
                .Execute(args, new ExtendedTextWrapper(Console.Out), Console.In);
            Console.ReadLine();
            return result;
        }
    }
}
