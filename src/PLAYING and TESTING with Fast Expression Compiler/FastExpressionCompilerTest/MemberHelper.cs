using FastExpressionCompiler;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace FastExpressionCompilerTest
{
    public class MemberHelper
    {
        private static readonly Type TypeOfObject = typeof(object);

        private static readonly ConcurrentDictionary<Type, MemberHelper[]> cache
            = new ConcurrentDictionary<Type, MemberHelper[]>();

        public string Name { get; set; }

        // Object obj => obj.Property;
        public Func<object, object> Getter { get; set; }

        public static MemberHelper[] GetMembers(Type type)
            => cache.GetOrAdd(type, _ =>
            {
                return type
                    .GetProperties()
                    .Cast<MemberInfo>()
                    .Concat(type.GetFields().Cast<MemberInfo>())
                    .Select(pr =>
                    {
                        // Object obj
                        var parameter = Expression.Parameter(TypeOfObject, "obj");

                        // (T)obj
                        var parameterConvert = Expression.Convert(parameter, type);

                        // ((T)obj).Property
                        var body = Expression.MakeMemberAccess(parameterConvert, pr);

                        // (object)((T)obj).Property
                        var convertedBody = Expression.Convert(body, TypeOfObject);

                        // Object obj => (object)((T)obj).Property
                        var lambda = Expression
                                .Lambda<Func<object, object>>(convertedBody, parameter);

                        var propertyGetterFunc = lambda.CompileFast();

                        return new MemberHelper
                        {
                            Name = pr.Name,
                            Getter = propertyGetterFunc
                        };
                    })
                    .ToArray();
            });
    }
}
