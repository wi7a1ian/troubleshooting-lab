using System;
using dep_missing_managed_dll;

namespace dep_missing_managed
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Console.WriteLine(SomeVeryImportantClass.SomeVeryImportantMethod());
            Console.WriteLine("Goodbye cruel World!");
        }
    }
}
