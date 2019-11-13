using System;
using System.Threading;
using System.Threading.Tasks;

namespace ex_thread_managed
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            
            Thread thread = new Thread(() => ProcessSomethingImportant());
            thread.Start();

            Thread.Sleep(TimeSpan.FromSeconds(1));
            thread.Join();

            Console.WriteLine("Goodbye cruel World!");
        }

        static void ProcessSomethingImportant()
        {
            Thread.Sleep(TimeSpan.FromMilliseconds(500));
            throw new NotImplementedException("Sorry!");
        }
    }
}
