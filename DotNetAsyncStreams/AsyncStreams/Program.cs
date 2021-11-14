using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AsyncStreams
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var transactions = ReadLines("transactions.csv");

            await foreach (var transaction in transactions)
            {
                Console.WriteLine($"Transaction from {transaction.SrcAccount} to {transaction.DstAccount}");
            }

            IAsyncEnumerable<IAsyncGrouping<string, Transaction>> grps =
                from t in transactions
                where t.usdAmount > 0
                group t by t.SrcAccount into g
                select g;

            await foreach (var grp in grps)
            {
                Console.WriteLine($"Account: {grp.Key} Count: {grp.CountAsync()} Sum: {grp.SumAsync(t=>t.usdAmount)}");
            }
        }

        public static async IAsyncEnumerable<Transaction> ReadLines(string path)
        {
            var lines = await System.IO.File.ReadAllLinesAsync(path);
            foreach (var line in lines)
            {
                string[] arr = line.Split(',');
                yield return new Transaction(
                    SrcAccount: arr[0],
                    DstAccount: arr[1],
                    timestamp: arr[2],
                    usdAmount: decimal.Parse(arr[3]));
            }
        }

        public record Transaction(string SrcAccount, string DstAccount, string timestamp, decimal usdAmount);
    }
}
