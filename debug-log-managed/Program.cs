using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace debug_log_managed
{
    class Program
    {
        internal static readonly CancellationTokenSource cancelSource = new CancellationTokenSource(TimeSpan.FromMinutes(5));
        internal static readonly TimeSpan logInterval = TimeSpan.FromMilliseconds(500);
        internal static readonly Stopwatch stopwatch = new Stopwatch();

        static void Main(string[] args) => MainAsync().Wait();

        static async Task MainAsync()
        {
            Trace.WriteLine("Started the app");

            stopwatch.Start();
            var cToken = cancelSource.Token;
            Console.CancelKeyPress += (_, e) => 
            {
                Trace.WriteLine("Cancellation requested by the client");
                cancelSource.Cancel(); 
                e.Cancel = true; 
            };

            while (!cToken.IsCancellationRequested)
            {
                try 
                { 
                    await Task.Delay(logInterval, cToken);

                    Trace.WriteLine("Do not kill me!");
                    Debug.WriteLine("Doing something important for {0}", stopwatch.Elapsed);
                }
                catch(Exception e)
                {
                    Trace.WriteLine("We got an exception :(");
                    Debug.WriteLine("Exception details: {0}", e);
                }
            }
         
            Trace.WriteLine("Finished the app");
        }
    }
}
