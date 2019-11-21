using System;

namespace BreakTheRuler
{
    public class MyClass<T1, T2>
    {
        public MyClass<MyClass<T1, T2>, MyClass<T1, T2>> HitMe { get; }
    }

    public class Program
    {
        public static void Main()
        {
            var instance = new MyClass<string, string>();

            var hitMe = instance
                .HitMe.HitMe.HitMe
                .HitMe.HitMe.HitMe
                .HitMe.HitMe.HitMe
                .HitMe.HitMe.HitMe
                .HitMe.HitMe.HitMe
                .HitMe.HitMe.HitMe
                .HitMe.HitMe.HitMe
                .HitMe.HitMe.HitMe
                .HitMe.HitMe.HitMe;
        }
    }
}
