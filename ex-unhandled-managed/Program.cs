using System;
using System.Threading;

namespace ex_unhandled_managed
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

    internal class SomeClass
    {
        internal void ProcessSomethingImportant()
        {
            throw new NotImplementedException("Sorry!");
        }
    }
}
