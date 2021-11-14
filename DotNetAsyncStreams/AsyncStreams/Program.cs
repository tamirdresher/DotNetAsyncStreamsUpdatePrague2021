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
                       
            IAsyncEnumerable<IAsyncGrouping<string, Transaction>> grps =
                from t in transactions
                where t.USDAmount > 0
                group t by t.SrcAccount into g
                select g;
            
            var firstAct = await grps.FirstAsync();

            await foreach (var transactionPair in firstAct.Buffer(2))
            {
                Console.WriteLine($"T1: {transactionPair[0].USDAmount} T2: {transactionPair[1].USDAmount}");
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
                    Timestamp: arr[2],
                    USDAmount: decimal.Parse(arr[3]));

                await Task.Delay(500);
            }
        }

        public record Transaction(string SrcAccount, string DstAccount, string Timestamp, decimal USDAmount);
    }
}
