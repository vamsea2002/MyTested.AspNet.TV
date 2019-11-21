using System;

namespace CoolLibrary
{
    internal static class SomeCoolClass
    {
        internal static string Name { get; set; }

        internal static string CoolMethod(int number, string text)
        {
            return number + text;
        }
    }
}
