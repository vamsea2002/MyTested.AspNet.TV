using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace ReflectionDelagatesDemo
{
    public class PropertyHelper
    {
        private static ConcurrentDictionary<string, Delegate> cache
            = new ConcurrentDictionary<string, Delegate>();

        private static readonly MethodInfo CallInnerDelegateMethod =
            typeof(PropertyHelper).GetMethod(nameof(CallInnerDelegate), BindingFlags.NonPublic | BindingFlags.Static);

        public static Func<object, TResult> MakeFastPropertyGetter<TResult>(PropertyInfo property)
            => (Func<object, TResult>)cache.GetOrAdd(property.Name, key =>
            {
                var getMethod = property.GetMethod;
                var declaringClass = property.DeclaringType;
                var typeOfResult = typeof(TResult);

                // Func<ControllerType, TResult>
                var getMethodDelegateType = typeof(Func<,>).MakeGenericType(declaringClass, typeOfResult);

                // c => c.Data
                var getMethodDelegate = getMethod.CreateDelegate(getMethodDelegateType);

                // CallInnerDelegate<ControllerType, TResult>
                var callInnerGenericMethodWithTypes = CallInnerDelegateMethod
                    .MakeGenericMethod(declaringClass, typeOfResult);

                // Func<object, TResult>
                var result = (Delegate)callInnerGenericMethodWithTypes.Invoke(null, new[] { getMethodDelegate });

                return result;
            });

        private static Func<object, TResult> CallInnerDelegate<TClass, TResult>(
            Func<TClass, TResult> deleg)
            => instance => deleg((TClass)instance);
    }
}
