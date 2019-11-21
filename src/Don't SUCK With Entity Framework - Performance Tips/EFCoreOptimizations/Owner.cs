using System.Collections.Generic;

namespace EFCoreOptimizations
{
    public class Owner
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public ICollection<Cat> Cats { get; set; } = new List<Cat>();
    }
}
