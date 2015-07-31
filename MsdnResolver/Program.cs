using System;
using System.Threading.Tasks;

namespace MsdnResolver
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            Run().Wait();
        }

        private static async Task Run()
        {
            // The documentation IDs are the same that the C#
            // uses in the documentation XML files.
            var documentationId = "T:System.Collections.Generic.List`1";

            var finder = new MsdnUrlFinder();
            var url = await finder.GetUrlAsync(documentationId);

            // Prints
            // http://msdn.microsoft.com/en-us/library/6sh2ey19
            Console.WriteLine(url);
        }
    }
}
