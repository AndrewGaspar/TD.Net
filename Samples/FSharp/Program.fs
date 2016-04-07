open System
open System.Threading.Tasks
open TD

let red init reducer input = (Enumerating.Reduce (input, init, reducer)).Value
let acc input = input |> red Unchecked.defaultof<_> (Accumulator.Checked())

let fib() = Standard.Multiplexing<_, _>(
                Core.Passing(),
                Standard.Feedback(
                    Standard.Sliding(2).Mapping(fun win -> win |> red Unchecked.defaultof<_> (Accumulator.Checked()))
                ))

[<EntryPoint>]
let main argv = 
//    [|0UL;1UL|].Reduce(Console.Out, fib().Apply(TextIO.WriteLineReducer<_>())) |> ignore
    [9.. -1 .. 0].ReduceAsync(
        Console.Out,
        Core.AsyncMapping(
            fun x -> Task.Delay(x * 100).ContinueWith(fun t -> x)
        ).Apply(TextIO.WriteLineReducer<_>().AsAsync())).Wait()
    0 // return an integer exit code
