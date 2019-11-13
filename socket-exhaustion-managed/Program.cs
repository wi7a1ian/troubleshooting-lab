using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace socket_exhaustion_managed
{
    class Program
    {
        internal static readonly CancellationTokenSource cancelSource = new CancellationTokenSource(TimeSpan.FromMinutes(5));
        internal static readonly TimeSpan readInterval = TimeSpan.FromMilliseconds(100);

        static void Main(string[] args) => MainAsync().Wait();

        static async Task MainAsync()
        {
            var cToken = cancelSource.Token;
            Console.CancelKeyPress += (_, e) => { cancelSource.Cancel(); e.Cancel = true; };

            try
            {
                for(int i = 0; i < 65535; ++i)
                {
                    using(var client = new HttpClient())
                    {
                        var result = await client.GetAsync("https://www.kldiscovery.com/", cToken);
                        Console.WriteLine(result.StatusCode);
                        await Task.Delay(readInterval, cToken);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e);
            }
        }
    }
}
