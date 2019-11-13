using System;
using System.Threading;
using System.Threading.Tasks;

namespace threadpool_starved_managed
{
    class Program
    {
        /// <summary>
        /// We put our system in a state where the threadpool is starved (i.e. all the threads are busy)
        /// 1. We enqueue 5 items per second into the global queue.
        /// 2. Each of those items, when executing, enqueues another item into the local queue and waits for it.
        /// 3. When a new thread is spawned by the threadpool, that thread will first look into its own local queue which is empty(since it’s newborn). 
        ///    Then it’ll pick an item from the global queue.
        /// 4. Since we enqueue into the global queue faster than the threadpool grows (5 items per second versus 1 thread per second), 
        ///    it’s completely impossible for the system to recover. Because of the priority induced by the usage of the global queue, 
        ///    the more threads we add, the more pressure we put on the system.
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Console.WriteLine(Environment.ProcessorCount);

            ThreadPool.SetMinThreads(8, 8); // threadpool is manually configured to start with 8 thread

            Task.Factory.StartNew(
                Producer,
                TaskCreationOptions.None);
            //TaskCreationOptions.LongRunning); // start in a dedicated thread

            // Whenever a threadpool thread is free, it will start looking into its local queue, and dequeue items in LIFO order.
            // If the local queue is empty, then the thread will look into the global queue and dequeue in FIFO order.
            // If the global queue is also empty, then the thread will look into the local queues of other threads and dequeue in FIFO order 
            // (to reduce the contention with the owner of the queue, which dequeues in LIFO order).
            Console.ReadLine();
        }

        static void Producer()
        {
            while (true)
            {
                Process();

                Thread.Sleep(200); // we start 5 tasks per second and each of those tasks will need an additional task

                // we need 10 threads in total to absorb the constant workload...
                // threadpool is manually configured to start with 8 threads, so we are 2 threads short
            }
        }

        static async Task Process()
        {
            await Task.Yield(); // yield on the default task scheduler to avoid blocking the caller, queues to the global queue

            var tcs = new TaskCompletionSource<bool>();

            // each of those tasks will need an additional task
            Task.Run(() =>
            {
                Thread.Sleep(1000); // enqueued in a local queue
                tcs.SetResult(true);
            });

            tcs.Task.Wait();

            Console.WriteLine("Ended - " + DateTime.Now.ToLongTimeString());
        }
    }
}
