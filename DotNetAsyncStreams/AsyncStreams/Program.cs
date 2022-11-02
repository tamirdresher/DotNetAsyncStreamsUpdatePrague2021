using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AsyncStreams
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var transactions = ReadLines("transactions.csv");

            var grps = from t in transactions
                       where t.usdAmount > 0
                       group t by t.SrcAccount into g
                       select g;

            foreach (var grp in grps)
            {
                Console.WriteLine($"Key: {grp.Key} Count: {grp.Count()}");
            }
        }

        public static IEnumerable<Transaction> ReadLines(string path)
        {
            var lines = System.IO.File.ReadAllLines(path);
            foreach (var line in lines)
            {
                string[] arr = line.Split(',');
                yield return new Transaction(
                    SrcAccount: arr[0],
                    DstAccount: arr[1],
                    timestamp:  arr[2],
                    usdAmount:  decimal.Parse(arr[3]));
            }
        }
        
        public record Transaction(string SrcAccount, string DstAccount, string timestamp, decimal usdAmount );
    }
}
