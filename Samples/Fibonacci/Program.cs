using System;
using TD;
using static TD.Core;
using static TD.Standard;

namespace Fibonacci
{
    class Program
    {
        static void Main(string[] args)
        {
            var fibonacci = 
                Multiplexing(
                    Passing<ulong>(),
                    Feedback(Sliding<ulong>(2).Mapping(window => checked(window[0] + window[1]))));

            new ulong[] { 0, 1 }.Reduce(Console.Out, fibonacci.Apply(TextIO.WriteLineReducer<ulong>()));
        }
    }
}
