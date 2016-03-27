// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

open TD;

let parse = Standard.Parsing<uint64>()
let inc = Core.Mapping(fun (x: uint64) -> x + 1UL)
let even = Core.Filtering(fun x -> x % 2UL = 0UL)

[<EntryPoint>]
let main argv = 
    System.Console.In.Transduce(System.Console.Out, parse.Compose(inc).Compose(even));
    0 // return an integer exit code
