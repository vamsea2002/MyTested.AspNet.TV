namespace NullableReferenceTypes
{
    using System.Diagnostics.CodeAnalysis;

    class Program
    {
        static void Main()
        {
            SomeGenericMethod<Cat>();
        }

        public static void SomeGenericMethod<TKey>()
            where TKey : notnull
        {
        }

        [return: NotNullIfNotNull("parameter")]
        public static string? Test(string? parameter)
        {
            return parameter ?? "Test";
        }

        public static string? Test(bool returnNotNullIfTrue)
        {
            return returnNotNullIfTrue ? "NotNull" : null;
        }
    }
}
