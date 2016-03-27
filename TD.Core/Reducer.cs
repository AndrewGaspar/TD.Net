using System;

namespace TD
{
    internal class FuncReducer<Reduction, From> : IReducer<Reduction, From>
    {
        public Func<Reduction, From, Terminator<Reduction>> ReducingFunction { get; private set; }

        public FuncReducer(Func<Reduction, From, Terminator<Reduction>> func)
        {
            ReducingFunction = func;
        }

        public Terminator<Reduction> Invoke(Reduction reduction, From value) => ReducingFunction(reduction, value);

        public Terminator<Reduction> Complete(Reduction reduction) => Terminator.Reduction(reduction);
    }

    /// <summary>
    /// A static class for functions related to creating Reducers.
    /// </summary>
    /// <seealso cref="IReducer{TReduction, TInput}"/>
    public static class Reducer
    {
        /// <summary>
        /// Produces an IReducer from an 
        /// <see cref="System.Linq.Enumerable.Aggregate{TSource, TAccumulate, TResult}(System.Collections.Generic.IEnumerable{TSource}, TAccumulate, Func{TAccumulate, TSource, TAccumulate}, Func{TAccumulate, TResult})"/>-like 
        /// function. This reducer should not be stateful outside of the state of Reduction.
        /// </summary>
        /// <typeparam name="TReduction">The type of the reduction.</typeparam>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <param name="func">A reducing/aggregating function.</param>
        /// <returns>An IReducer that feeds input to the supplied function.</returns>
        public static IReducer<TReduction, TInput> Make<TReduction, TInput>(Func<TReduction, TInput, TReduction> func) =>
            new FuncReducer<TReduction, TInput>((reduction, input) => Terminator.Reduction(func(reduction, input)));

        /// <summary>
        /// Produces an IReducer from an 
        /// <see cref="System.Linq.Enumerable.Aggregate{TSource, TAccumulate, TResult}(System.Collections.Generic.IEnumerable{TSource}, TAccumulate, Func{TAccumulate, TSource, TAccumulate}, Func{TAccumulate, TResult})"/>-like 
        /// function. This reducer should not be stateful outside of the state of Reduction.
        /// </summary>
        /// <typeparam name="TReduction">The type of the reduction.</typeparam>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <param name="func">A reducing/aggregating function.</param>
        /// <returns>An IReducer that feeds input to the supplied function.</returns>
        public static IReducer<TReduction, TInput> Make<TReduction, TInput>(Func<TReduction, TInput, Terminator<TReduction>> func) =>
            new FuncReducer<TReduction, TInput>(func);
    }
}
