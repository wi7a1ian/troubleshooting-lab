using System;
using System.Threading.Tasks;
using System.Linq;
using Serilog;
using System.Threading;
using System.Diagnostics;

namespace windows_log_managed
{
    class Program
    {
        internal static readonly CancellationTokenSource cancelSource = new CancellationTokenSource(TimeSpan.FromMinutes(5));
        internal static readonly TimeSpan logInterval = TimeSpan.FromMilliseconds(500);
        internal static readonly Stopwatch stopwatch = new Stopwatch();

        static void Main(string[] args) => MainAsync().Wait();

        static async Task MainAsync()
        {
            Log.Logger = new LoggerConfiguration()
                //.Enrich.FromLogContext()
                .WriteTo.EventLog("Some App", manageEventSource: true)
                .CreateLogger();

            Log.Information("Started the app");

            stopwatch.Start();
            var cToken = cancelSource.Token;
            Console.CancelKeyPress += (_, e) =>
            {
                Log.Information("Cancellation requested by the client");
                cancelSource.Cancel();
                e.Cancel = true;
            };

            while (!cToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(logInterval, cToken);
                    Log.Information("Do not kill me!");
                    Log.Debug("Doing something important for {0}", stopwatch.Elapsed);
                }
                catch (Exception e) when (IsCancellationException(e))
                {
                    Log.Warning("Operation was forcibly cancelled");
                }
                catch (Exception e)
                {
                    Log.Error(e, "Some unecpected exception happened!");
                }
            }

            Log.Information("Finished the app");
            Log.CloseAndFlush();
        }

        private static bool IsCancellationException(Exception e) =>
            e is OperationCanceledException || e is TaskCanceledException || (e is AggregateException && IsCancellationException(e as AggregateException));

        private static bool IsCancellationException(AggregateException e) =>
            e.InnerExceptions.Any(ie => IsCancellationException(ie));
    }
}