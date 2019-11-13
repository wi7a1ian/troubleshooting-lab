using System;
using System.Buffers;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace io_random_managed
{
    class Program
    {
        internal static readonly CancellationTokenSource cancelSource = new CancellationTokenSource(TimeSpan.FromMinutes(5));
        internal static readonly TimeSpan readInterval = TimeSpan.FromMilliseconds(100);
        internal static readonly string testFile = "X:/DRTOOLS/Prelim/WzajacTemp/sequential-file.dat";
        internal const int oneMb = 1024 * 1024;

        static void Main(string[] args) => MainAsync().Wait();

        static async Task MainAsync()
        {
            var cToken = cancelSource.Token;
            Console.CancelKeyPress += (_, e) => { cancelSource.Cancel(); e.Cancel = true; };

            try
            {
                using (IMemoryOwner<byte> mOwner = MemoryPool<byte>.Shared.Rent(oneMb))
                using (var sr = File.OpenRead(testFile))
                {
                    var rng = new Random();
                    while (!cToken.IsCancellationRequested && await sr.ReadAsync(mOwner.Memory.Slice(0, oneMb), cToken) > 0)
                    {
                        await Task.Delay(readInterval, cToken);

                        sr.Position -= rng.Next(0, oneMb/100);
                        //sr.BaseStream.Position -= rng.Next(0, oneMb/100);
                        //sr.DiscardBufferedData();
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
