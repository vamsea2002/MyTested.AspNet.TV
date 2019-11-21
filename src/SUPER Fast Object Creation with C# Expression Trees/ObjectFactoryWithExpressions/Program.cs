using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace ObjectFactoryWithExpressions
{
    public class Program
    {
        public static void Main()
        {
            var list = new List<Cat>();

            var stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < 1; i++)
            {
                var cat = Activator.CreateInstance<Cat>();

                list.Add(cat);
            }

            Console.WriteLine($"{stopwatch.Elapsed} - Activator - No parameters in constructor");
            Console.WriteLine(list.Count);

            list = new List<Cat>();
            New<Cat>.Instance();
            stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < 1; i++)
            {
                var cat = New<Cat>.Instance();

                list.Add(cat);
            }

            Console.WriteLine($"{stopwatch.Elapsed} - Expression Trees - No parameters in constructor");
            Console.WriteLine(list.Count);

            list = new List<Cat>();
            stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < 1; i++)
            {
                var cat = (Cat)Activator.CreateInstance(typeof(Cat), "My Cool Cat", 2);

                list.Add(cat);
            }

            Console.WriteLine($"{stopwatch.Elapsed} - Activator - 2 parameters in constructor");
            Console.WriteLine(list.Count);

            list = new List<Cat>();
            ObjectFactory.CreateInstance(typeof(Cat), "My Cool Cat");
            stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < 1; i++)
            {
                var cat = (Cat)ObjectFactory.CreateInstance(typeof(Cat), "My Cool Cat", 2);

                list.Add(cat);
            }

            Console.WriteLine($"{stopwatch.Elapsed} - Expression Trees - 2 parameters in constructor");
            Console.WriteLine(list.Count);
        }
    }
}
