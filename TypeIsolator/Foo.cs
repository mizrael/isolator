using System;
using System.Threading;

namespace TypeIsolator
{
    public class Foo
    {
        public void WhereAmI()
        {
            Console.WriteLine($"my app domain is: {AppDomain.CurrentDomain.FriendlyName}");
        }
        
    }
}