namespace ObjectFactoryWithExpressions
{
    public class Cat
    {
        public Cat()
        {
        }

        public Cat(string name)
            => this.Name = name;

        public Cat(string name, int age)
        {
            this.Name = name;
            this.Age = age;
        }

        public string Name { get; set; }

        public int Age { get; set; }
    }
}
