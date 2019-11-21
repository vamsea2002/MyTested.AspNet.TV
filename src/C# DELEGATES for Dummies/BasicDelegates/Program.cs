using System;

namespace BasicDelegates
{
    public class Cat
    {
        public string Name { get; set; }

        public void SomeCatMethod(int number)
        {

        }
    }

    public class Program
    {
        public static void Main()
        {
            var voidDelegate = new MyVoidDelegate(PrintInteger);

            voidDelegate += SomeMethod;
            voidDelegate += PrintInteger;

            var cat = new Cat();

            voidDelegate += cat.SomeCatMethod;

            voidDelegate(100);

            Console.WriteLine(voidDelegate.Target?.GetType().Name);

            PassSomeDelegate(voidDelegate);
        }

        public static void PassSomeDelegate(MyVoidDelegate del)
        {
            del?.Invoke(5);
        }

        public static void PrintInteger(int number)
        {
            Console.WriteLine(number);
        }

        public static void SomeMethod(int myInt)
        {
            Console.WriteLine(myInt + 10);
        }

        public static void SomeMethodWithString(string text)
        {

        }

        public static string SomeOtherMethod(int x, int y)
        {
            return x + y.ToString();
        }
    }
}
