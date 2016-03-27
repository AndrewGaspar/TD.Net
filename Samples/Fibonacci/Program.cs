using System;
using TD;
using static TD.Core;
using static TD.Standard;

namespace Fibonacci
{
    class Program
    {
        private static ITransducer<T, T> Fibonacci<T>() where T : struct =>
            Multiplexing(
                Passing<T>(),
                Feedback(
                    Sliding<T>(2).Mapping(window => window.Reduce(default(T), Accumulator.Checked<T>()).Value)
                )
            );

        static void Main(string[] args)
        {
            new ulong[] { 0, 1 }.Reduce(Console.Out, Fibonacci<ulong>().Apply(TextIO.WriteLineReducer<ulong>()));
        }
    }
}
