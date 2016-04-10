using System;
using TD;
using static TD.Core;
using static TD.Standard;

namespace Fibonacci
{
    class Program
    {
        private static ITransducer<T, T> Fibonacci<T>() where T : struct =>
            Multiplexing(Passing<T>(), Feedback(Sliding<T>(2).CheckedSumming()));

        static void Main(string[] args) =>
            new[] { 0ul, 1ul }.Reduce(Console.Out, Fibonacci<ulong>().Apply(TextIO.WriteLineReducer<ulong>()));
    }
}
