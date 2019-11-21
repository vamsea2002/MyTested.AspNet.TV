using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ReflectionDelagatesDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            // get controller assembly
            // get all types which name ends with "Controller"

            var homeController = new HomeController();
            var homeControllerType = homeController.GetType();

            var property = homeControllerType.GetProperties()
                .FirstOrDefault(pr => pr.IsDefined(typeof(DataAttribute), true));

            var getMethod = property.GetMethod;

            var stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < 100000; i++)
            {
                var dict = (IDictionary<string, object>)getMethod.Invoke(homeController, Array.Empty<object>());
            }

            Console.WriteLine(stopwatch.Elapsed);

            var deleg = PropertyHelper
                .MakeFastPropertyGetter<IDictionary<string, object>>(property);

            stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < 100000; i++)
            {
                var dict = deleg(homeController);
            }

            Console.WriteLine(stopwatch.Elapsed);
        }
    }
}
