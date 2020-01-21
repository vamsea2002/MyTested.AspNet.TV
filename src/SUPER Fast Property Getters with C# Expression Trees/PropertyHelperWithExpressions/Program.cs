namespace PropertyHelperWithExpressions
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    public class Program
    {
        public static void Main()
        {
            // return RedirectToAction("Index", new { id = 5, query = "Text" });
            // Dictionary { ["id"] = 5, ["query"] = "Text" }

            var obj = new { id = 5, query = "Text" };

            var dict = new Dictionary<string, object>();

            var stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < 1000; i++)
            {
                PropertyHelper
                    .GetProperties(obj.GetType())
                    .Select(pr => new
                    {
                        pr.Name,
                        Value = pr.Getter(obj)
                    })
                    .ToList()
                    .ForEach(pr => dict[pr.Name] = pr.Value);
            }

            Console.WriteLine($"{stopwatch.Elapsed} - Normal Expression Tree");
            Console.WriteLine(dict.Count);

            dict = new Dictionary<string, object>();

            stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < 1000; i++)
            {
                PropertyHelperFast
                    .GetProperties(obj.GetType())
                    .Select(pr => new
                    {
                        pr.Name,
                        Value = pr.Getter(obj)
                    })
                    .ToList()
                    .ForEach(pr => dict[pr.Name] = pr.Value);
            }

            Console.WriteLine($"{stopwatch.Elapsed} - Fast Expression Tree");
            Console.WriteLine(dict.Count);
        }
    }
}
