using System.Collections.Generic;

namespace ReflectionDelagatesDemo
{
    public class HomeController
    {
        public HomeController()
            => this.Data = new Dictionary<string, object>
            {
                ["Name"] = "My Tested ASP.NET"
            };

        [Data]
        public IDictionary<string, object> Data { get; set; }
    }
}
