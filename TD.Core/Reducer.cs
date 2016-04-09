using System;
using System.Threading.Tasks;
using static TD.Terminator;

namespace TD
{
    internal class FuncReducer<Reduction, From> : IReducer<Reduction, From>
    {
        private readonly Func<Reduction, From, Terminator<Reduction>> ReducingFunction;

        public FuncReducer(Func<Reduction, From, Terminator<Reduction>> func)
        {
            ReducingFunction = func;
        }

        public Terminator<Reduction> Invoke(Reduction reduction, From value) => ReducingFunction(reduction, value);

        public Terminator<Reduction> Complete(Reduction reduction) => Reduction(reduction);
    }

    internal class FuncAsyncReducer<TReduction, TInput> : IAsyncReducer<TReduction, TInput>
    {
        private readonly Func<TReduction, TInput, Task<Terminator<TReduction>>> ReducingFunction;

        public FuncAsyncReducer(Func<TReduction, TInput, Task<Terminator<TReduction>>> func)
        {
            ReducingFunction = func;
        }

        public Task<Terminator<TReduction>> CompleteAsync(TReduction reduction) =>
            Task.FromResult(Reduction(reduction));

        public Task<Terminator<TReduction>> InvokeAsync(TReduction reduction, TInput value) =>
            ReducingFunction(reduction, value);
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
            new FuncReducer<TReduction, TInput>((reduction, input) => Reduction(func(reduction, input)));

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

        public static IAsyncReducer<TReduction, TInput> AsyncMake<TReduction, TInput>(
            Func<TReduction, TInput, Task<TReduction>> func) =>
                new FuncAsyncReducer<TReduction, TInput>(
                    async (reduction, input) => Reduction(await func(reduction, input)));

        public static IAsyncReducer<TReduction, TInput> AsyncMake<TReduction, TInput>(
            Func<TReduction, TInput, Task<Terminator<TReduction>>> func) =>
                new FuncAsyncReducer<TReduction, TInput>(func);
    }
}
