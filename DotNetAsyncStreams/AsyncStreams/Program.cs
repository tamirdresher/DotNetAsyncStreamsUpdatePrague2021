using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Reactive.Linq;

namespace AsyncStreams
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var first = AsyncEnumerable.Range(1, 10)
                .Do(async x => await Task.Delay(1000))
                .ToObservable();
            var second = AsyncEnumerable.Range(20, 10)
                .Do(async x => await Task.Delay(500))
                .ToObservable();

            var combined =
                first.CombineLatest(second)
                    .Select(pair=>pair.First+pair.Second)
                    .ToAsyncEnumerable();

            await foreach (var x in combined)
            {
                Console.WriteLine(x);
            }
        }

    }
}
