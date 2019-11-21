using System;

namespace ActionAndFuncDelegates
{
    public class Cat
    {
        public string Name { get; set; }

        public void SayMew()
        {
            Console.WriteLine("Mew");
        }
    }

    public class Program
    {
        public static void Main()
        {
            Action<int, string, bool> action = SomeMethod;

            action += (x, y, z) => Console.WriteLine(x);

            action(10, "text", true);

            Func<string, bool, int> func = SomeMethod;

            Func<int, int, int> sumFunc = (x, y) => x + y;

            Console.WriteLine(sumFunc(5, 10));

            Action<Cat> catAction = cat => cat.SayMew();

            var cat = new Cat
            {
                Name = "My Tested ASP.NET"
            };

            Calculate(cat => cat.Name);
            Calculate(cat => cat.Name + " Is Cool!");
        }

        public static void Calculate(Func<Cat, string> func)
        {
            var cat = new Cat
            {
                Name = "My Tested ASP.NET"
            };

            var result = func(cat);

            Console.WriteLine(result);
        }

        public static int SomeMethod(string text, bool someBool)
        {
            Console.WriteLine("Calling." + text);
            return 100;
        }

        public static void SomeMethod(int number, string text, bool someBool)
        {
            Console.WriteLine("Test");
        }
    }
}
