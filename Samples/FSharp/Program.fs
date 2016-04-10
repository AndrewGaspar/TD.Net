open System
open System.Threading.Tasks
open TD

let red init reducer input = (Enumerating.Reduce (input, init, reducer)).Value
let acc input = input |> red Unchecked.defaultof<_> (Accumulator.Checked())

let fib() = Standard.Multiplexing<_, _>(
                Core.Passing(),
                Standard.Feedback(
                    Standard.Sliding(2).UncheckedSumming()
                ))

[<EntryPoint>]
let main argv = 
//    [|0UL;1UL|].Reduce(Console.Out, fib().Apply(TextIO.WriteLineReducer<_>())) |> ignore
    [9.. -1 .. 0].ReduceAsync(
        Console.Out,
        Core.Mapping(
            fun x -> Task.Delay(x * 100).ContinueWith(fun t -> x)
        ).Awaiting().Mapping(fun x -> x.ToString()).AsyncWriteLineReducer()).Wait();
    0 // return an integer exit code
