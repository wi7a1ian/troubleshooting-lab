using System;
using System.Threading;
using ex_unhandled_module_managed_dll;

namespace ex_unhandled_module_managed
{
    class Program
    {
        static void Main(string[] args)
        {
            var s = new SomeClass();
            Thread.Sleep(TimeSpan.FromSeconds(1));
            s.ProcessSomethingImportant();
        }
    }
}
