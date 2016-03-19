using System;

namespace TD
{
    internal class FuncReducer<Reduction, From> : IReducer<Reduction, From>
    {
        public Func<Reduction, From, Reduction> ReducingFunction { get; private set; }

        public FuncReducer(Func<Reduction, From, Reduction> func)
        {
            ReducingFunction = func;
        }

        public Terminator<Reduction> Invoke(Reduction reduction, From value) =>
            Terminator.Reduction(ReducingFunction(reduction, value));

        public Terminator<Reduction> Complete(Reduction reduction) => Terminator.Reduction(reduction);
    }

    public static class Reducer
    {
        public static IReducer<Reduction, Input> Make<Reduction, Input>(Func<Reduction, Input, Reduction> func) =>
            new FuncReducer<Reduction, Input>(func);
    }
}
