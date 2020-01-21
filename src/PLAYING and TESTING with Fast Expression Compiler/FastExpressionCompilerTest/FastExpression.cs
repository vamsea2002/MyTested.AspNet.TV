using FastExpressionCompiler.LightExpression;
using System;

namespace FastExpressionCompilerTest
{
    public class FastExpression
    {
        public string BuildFastExpression()
        {
            // cat => cat.SayMew(test)
            var param = Expression.Parameter(typeof(Cat), "cat");
            var variable = Expression.Constant("test");
            var body = Expression.Call(param, typeof(Cat).GetMethod(nameof(Cat.SayMew)), variable);

            var lambda = Expression.Lambda<Func<Cat, string>>(body, param);

            var func = lambda.CompileFast();

            return func(new Cat());
        }
    }
}
