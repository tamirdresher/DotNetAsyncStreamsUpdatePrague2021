using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.CompilerServices;

namespace AsyncStreams
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var transactions = ReadLines("transactions.csv");
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(1000);

            await foreach (var transaction in transactions.WithCancellation(cts.Token).ConfigureAwait(false))
            {
                Console.WriteLine($"Transaction from {transaction.SrcAccount} to {transaction.DstAccount}");
            }

            IAsyncEnumerable<IAsyncGrouping<string, Transaction>> grps =
                from t in transactions
                where t.usdAmount > 0
                group t by t.SrcAccount into g
                select g;

            grps = grps.SelectAwait(async x =>
            {
                await Task.Delay(500);
                return x; 
            });

            await foreach (var grp in grps)
            {
                Console.WriteLine($"Account: {grp.Key} Count: {await grp.CountAsync()} Sum: {await grp.SumAsync(t => t.usdAmount)}");
            }
        }

        public static async IAsyncEnumerable<Transaction> ReadLines(string path, [EnumeratorCancellation] CancellationToken token = default)
        {
            var lines = await System.IO.File.ReadAllLinesAsync(path);
            foreach (var line in lines)
            {
                token.ThrowIfCancellationRequested();
                string[] arr = line.Split(',');
                yield return new Transaction(
                    SrcAccount: arr[0],
                    DstAccount: arr[1],
                    timestamp: arr[2],
                    usdAmount: decimal.Parse(arr[3]));

                await Task.Delay(500);
            }
        }

        public record Transaction(string SrcAccount, string DstAccount, string timestamp, decimal usdAmount);
    }
}
