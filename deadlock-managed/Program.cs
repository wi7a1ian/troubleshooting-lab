using System;
using System.Threading;
using System.Threading.Tasks;

namespace deadlock_managed
{
    class Program
    {
        internal static object lockObj1 = new object();
        internal static object lockObj2 = new object();

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var t1 = Task.Run( () => ProcessSomethingImportant1());
            var t2 = Task.Run( () => ProcessSomethingImportant2());

            Task.WaitAll(t1, t2);
            Console.WriteLine("Goodbye cruel World!");
        }

        static void ProcessSomethingImportant1()
        {
            lock (lockObj1)
            {
                Console.WriteLine("ProcessSomethingImportant1 started");
                Thread.Sleep(100);
                lock (lockObj2)
                { 
                    Thread.Sleep(100);
                }
                Console.WriteLine("ProcessSomethingImportant1 finished");
            }
        }

        static void ProcessSomethingImportant2()
        {
            lock (lockObj2)
            {
                Console.WriteLine("ProcessSomethingImportant2 started");
                Thread.Sleep(100);
                lock (lockObj1)
                {
                    Thread.Sleep(100);
                }
                Console.WriteLine("ProcessSomethingImportant2 finished");
            }
        }
    }
}
