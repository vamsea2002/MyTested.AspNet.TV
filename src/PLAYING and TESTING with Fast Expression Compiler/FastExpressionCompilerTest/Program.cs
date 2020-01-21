namespace FastExpressionCompilerTest
{
    using FastExpressionCompiler;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq.Expressions;
    using System.Reflection;

    public class Program
    {
        public static void Main()
        {
            var test = "Test";

            Expression<Func<Cat, string>> expression = cat => cat.SayMew(test);

            var list = new List<string>();

            var stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < 10000; i++)
            {
                var body = expression.Body as MethodCallExpression;
                var argument = body.Arguments[0];

                // () => (object)test;
                var converted = Expression.Convert(argument, typeof(object));
                var lambda = Expression.Lambda<Func<object>>(converted);

                var func = lambda.Compile();

                list.Add(func() as string);
            }

            Console.WriteLine($"{stopwatch.Elapsed} - Extracting with normal expression and convert");
            Console.WriteLine(list.Count);

            list = new List<string>();

            stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < 10000; i++)
            {
                var body = expression.Body as MethodCallExpression;
                var argument = body.Arguments[0];

                // () => (object)test;
                var converted = Expression.Convert(argument, typeof(object));
                var lambda = Expression.Lambda<Func<object>>(converted);

                var func = lambda.CompileFast();

                list.Add(func() as string);
            }

            Console.WriteLine($"{stopwatch.Elapsed} - Extracting with fast compiled expression and convert");
            Console.WriteLine(list.Count);

            list = new List<string>();

            stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < 10000; i++)
            {
                var body = expression.Body as MethodCallExpression;
                var argument = body.Arguments[0];

                var argumentMember = argument as MemberExpression;
                var closureClass = argumentMember.Expression as ConstantExpression;
                var closureClassValue = closureClass.Value;

                var fieldInfo = argumentMember.Member as FieldInfo;
                var value = fieldInfo.GetValue(closureClassValue);

                list.Add(value as string);
            }

            Console.WriteLine($"{stopwatch.Elapsed} - Extracting with reflection");
            Console.WriteLine(list.Count);

            list = new List<string>();

            var bodyCold = expression.Body as MethodCallExpression;
            var argumentCold = bodyCold.Arguments[0];

            var argumentMemberCold = argumentCold as MemberExpression;
            var closureClassCold = argumentMemberCold.Expression as ConstantExpression;
            var closureClassValueCold = closureClassCold.Value;

            MemberHelper.GetMembers(closureClassValueCold.GetType());

            stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < 10000; i++)
            {
                var body = expression.Body as MethodCallExpression;
                var argument = body.Arguments[0];

                var argumentMember = argument as MemberExpression;
                var closureClass = argumentMember.Expression as ConstantExpression;
                var closureClassValue = closureClass.Value;

                var members = MemberHelper.GetMembers(closureClassValue.GetType());

                var value = members[0].Getter(closureClassValue);

                list.Add(value as string);
            }

            Console.WriteLine($"{stopwatch.Elapsed} - Extracting with reflection and compiled delegates");
            Console.WriteLine(list.Count);

            list = new List<string>();

            stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < 10000; i++)
            {
                var slowExpression = new SlowExpression();

                var value = slowExpression.BuildSlowExpression();

                list.Add(value);
            }

            Console.WriteLine($"{stopwatch.Elapsed} - Building with slow expression");
            Console.WriteLine(list.Count);

            list = new List<string>();

            stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < 10000; i++)
            {
                var slowExpression = new SlowExpressionWithFastCompile();

                var value = slowExpression.BuildSlowExpression();

                list.Add(value);
            }

            Console.WriteLine($"{stopwatch.Elapsed} - Building with slow expression and fast compile");
            Console.WriteLine(list.Count);

            list = new List<string>();

            stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < 10000; i++)
            {
                var slowExpression = new FastExpression();

                var value = slowExpression.BuildFastExpression();

                list.Add(value);
            }

            Console.WriteLine($"{stopwatch.Elapsed} - Building with fast expression");
            Console.WriteLine(list.Count);
        }
    }
}
