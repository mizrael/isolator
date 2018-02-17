using System;

namespace TypeIsolator
{
    class Program
    {
        static void Main(string[] args)
        {
            var localFoo = new Foo();
            localFoo.WhereAmI();

            using (var isolatedFoo = new Isolator(typeof(Foo), "WhereAmI"))
                isolatedFoo.Run();
            
            Console.ReadLine();
        }
    }
}
