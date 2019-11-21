using System;

namespace SimpleEvents
{
    public class Program
    {
        public static void Main()
        {
            var cat = new Cat
            {
                Id = 1,
                Name = "My Tested ASP.NET",
                Health = 100
            };

            cat.OnHealthChanged += CatOnHealthChanged;
            cat.OnHealthChanged += CatIsDead;

            cat.Health = 200;

            for (int i = 0; i < 10; i++)
            {
                cat.Health -= 20;
            }
        }

        private static void CatIsDead(object sender, int health)
        {
            var cat = (Cat)sender;
            if (cat.Health <= 0)
            {
                Console.WriteLine($"{cat.Name} is no longer alive... :(");
            }
        }

        private static void CatOnHealthChanged(object sender, int health)
        {
            var cat = (Cat)sender;
            Console.WriteLine($"{cat.Name} has new health: {health}");
        }
    }
}
