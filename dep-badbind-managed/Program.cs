using System;
using System.Net.Http;

namespace dep_badbind_managed
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var client = new HttpClient(); // v4.2.0.0
            Console.WriteLine("Goodbye cruel World!");
        }
    }
}
