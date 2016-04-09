using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static TD.Terminator;

namespace TD
{
    /// <summary>
    /// Factory functions for creating components of a switching transducer.
    /// </summary>
    public static class TransducerSwitch
    {
        /// <summary>
        /// Creates a switch for a Switching transducer that pushes results into the supplied transducer if
        /// an unmatched input matches the supplied test.
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="test">The test.</param>
        /// <param name="transducer">The transducer.</param>
        /// <returns>A switch for a Switching transducer.</returns>
        public static TransducerSwitch<TInput, TResult> Create<TInput, TResult>(Predicate<TInput> test, ITransducer<TInput, TResult> transducer) =>
            new TransducerSwitch<TInput, TResult>(test, transducer);

        /// <summary>
        /// Creates the default switch for a Switching transducer. Any input that isn't captured by a
        /// previous switch in the Switching transducer will fall into this transducer.
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="transducer">The transducer.</param>
        /// <returns>A default switch for a Switching transducer.</returns>
        public static TransducerSwitch<TInput, TResult> Default<TInput, TResult>(ITransducer<TInput, TResult> transducer) =>
            Create(_ => true, transducer);
    }

    /// <summary>
    /// A switch to be used as part of a Switching transducer.
    /// </summary>
    /// <typeparam name="TInput">The type of the input.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public class TransducerSwitch<TInput, TResult>
    {
        internal TransducerSwitch(Predicate<TInput> test, ITransducer<TInput, TResult> transducer)
        {
            Test = test;
            Transducer = transducer;
        }

        /// <summary>
        /// Gets the switch's test.
        /// </summary>
        /// <value>
        /// The switch's test.
        /// </value>
        public Predicate<TInput> Test { get; private set; }

        /// <summary>
        /// Gets the switch's transducer.
        /// </summary>
        /// <value>
        /// The transducer.
        /// </value>
        public ITransducer<TInput, TResult> Transducer { get; private set; }
    }

    internal class Switching<TInput, TResult> : ITransducer<TInput, TResult>
    {
        private class SplittingReducer<TReduction> : IReducer<TReduction, TInput>
        {
            private class ReducerOption
            {
                public ReducerOption(Predicate<TInput> test, IReducer<TReduction, TInput> reducer)
                {
                    Test = test;
                    IsTerminated = false;
                    Reducer = reducer;
                }

                public Predicate<TInput> Test { get; private set; }
                public bool IsTerminated { get; set; }
                public IReducer<TReduction, TInput> Reducer { get; set; }
            }

            private class JoiningReducer : IReducer<TReduction, TResult>
            {
                private readonly SplittingReducer<TReduction> Splitter;
                private readonly IReducer<TReduction, TResult> Next;

                public JoiningReducer(
                    SplittingReducer<TReduction> splitter,
                    IReducer<TReduction, TResult> next)
                {
                    Splitter = splitter;
                    Next = next;
                }

                private bool CheckTermination(Terminator<TReduction> terminator)
                {
                    if (terminator.IsTerminated)
                    {
                        foreach (var reducer in Splitter.Reducers)
                        {
                            reducer.IsTerminated = true;
                        }

                        return true;
                    }

                    return false;
                }

                public Terminator<TReduction> Complete(TReduction reduction)
                {
                    var terminator = Next.Complete(reduction);

                    var terminated = CheckTermination(terminator);

                    return Reduction(terminator.Value, terminated: terminated);
                }

                public Terminator<TReduction> Invoke(TReduction reduction, TResult value)
                {
                    var terminator = Next.Invoke(reduction, value);

                    var terminated = CheckTermination(terminator);

                    return Reduction(terminator.Value, terminated: terminated);
                }
            }

            private readonly IList<ReducerOption> Reducers;
            private readonly IReducer<TReduction, TResult> Next;

            public SplittingReducer(
                IList<TransducerSwitch<TInput, TResult>> transducers,
                IReducer<TReduction, TResult> next)
            {
                Reducers = transducers
                    .Select(tSwitch =>
                        new ReducerOption(
                            tSwitch.Test, 
                            tSwitch.Transducer.Apply(new JoiningReducer(this, next))))
                    .ToList();
                Next = next;
            }

            private ReducerOption GetMatchingReducer(TInput value) =>
                Reducers.First(reducer => reducer.Test(value));

            public Terminator<TReduction> Invoke(TReduction reduction, TInput value)
            {
                var reducer = GetMatchingReducer(value);

                if (!reducer.IsTerminated)
                {
                    var terminator = reducer.Reducer.Invoke(reduction, value);

                    if (terminator.IsTerminated)
                    {
                        reducer.IsTerminated = true;
                    }

                    reduction = terminator.Value;
                }

                return Reduction(reduction, terminated: Reducers.All(red => red.IsTerminated));
            }

            public Terminator<TReduction> Complete(TReduction reduction) =>
                Reducers.Where(reducer => !reducer.IsTerminated)
                        .Aggregate(Reduction(reduction), (term, reducer) =>
                            reducer.Reducer.Complete(reduction));
        }

        private class SplittingAsyncReducer<TReduction> : IAsyncReducer<TReduction, TInput>
        {
            private class AsyncReducerOption
            {
                public AsyncReducerOption(Predicate<TInput> test, IAsyncReducer<TReduction, TInput> reducer)
                {
                    Test = test;
                    IsTerminated = false;
                    Reducer = reducer;
                }

                public Predicate<TInput> Test { get; private set; }
                public bool IsTerminated { get; set; }
                public IAsyncReducer<TReduction, TInput> Reducer { get; set; }
            }

            class JoiningReducer : IAsyncReducer<TReduction, TResult>
            {
                private readonly SplittingAsyncReducer<TReduction> Splitter;
                private readonly IAsyncReducer<TReduction, TResult> Next;

                public JoiningReducer(
                    SplittingAsyncReducer<TReduction> splitter,
                    IAsyncReducer<TReduction, TResult> next)
                {
                    Splitter = splitter;
                    Next = next;
                }

                private bool CheckTermination(Terminator<TReduction> terminator)
                {
                    if (terminator.IsTerminated)
                    {
                        foreach (var reducer in Splitter.Reducers)
                        {
                            reducer.IsTerminated = true;
                        }

                        return true;
                    }

                    return false;
                }

                public async Task<Terminator<TReduction>> CompleteAsync(TReduction reduction)
                {
                    var terminator = await Next.CompleteAsync(reduction).ConfigureAwait(false);

                    var terminated = CheckTermination(terminator);

                    return Reduction(terminator.Value, terminated: terminated);
                }

                public async Task<Terminator<TReduction>> InvokeAsync(TReduction reduction, TResult value)
                {
                    var terminator = await Next.InvokeAsync(reduction, value).ConfigureAwait(false);

                    var terminated = CheckTermination(terminator);

                    return Reduction(terminator.Value, terminated: terminated);
                }
            }

            private readonly IList<AsyncReducerOption> Reducers;
            private readonly IAsyncReducer<TReduction, TResult> Next;

            public SplittingAsyncReducer(
                IList<TransducerSwitch<TInput, TResult>> transducers,
                IAsyncReducer<TReduction, TResult> next)
            {
                Reducers = transducers
                    .Select(tSwitch =>
                        new AsyncReducerOption(
                            tSwitch.Test,
                            tSwitch.Transducer.Apply(new JoiningReducer(this, next))))
                    .ToList();
                Next = next;
            }

            private AsyncReducerOption GetMatchingReducer(TInput value) =>
                Reducers.First(reducer => reducer.Test(value));

            public async Task<Terminator<TReduction>> InvokeAsync(TReduction reduction, TInput value)
            {
                var reducer = GetMatchingReducer(value);

                if (!reducer.IsTerminated)
                {
                    var terminator = await reducer.Reducer.InvokeAsync(reduction, value);

                    if (terminator.IsTerminated)
                    {
                        reducer.IsTerminated = true;
                    }

                    reduction = terminator.Value;
                }

                return Reduction(reduction, terminated: Reducers.All(red => red.IsTerminated));
            }

            public async Task<Terminator<TReduction>> CompleteAsync(TReduction reduction)
            {
                var filteringUnterminated = Core.Filtering<AsyncReducerOption>(option => !option.IsTerminated);

                var invokesCompletion = Reducer.AsyncMake<Terminator<TReduction>, AsyncReducerOption>((terminator, option) =>
                    option.Reducer.CompleteAsync(terminator.Value));

                var reduceTask = Reducers.ReduceAsync(
                    Reduction(reduction), filteringUnterminated.Apply(invokesCompletion));

                return (await reduceTask.ConfigureAwait(false)).Value;
            }
        }


        private readonly IList<TransducerSwitch<TInput, TResult>> Transducers;
        public Switching(IList<TransducerSwitch<TInput, TResult>> transducers)
        {
            Transducers = transducers;
        }

        public IReducer<TReduction, TInput> Apply<TReduction>(IReducer<TReduction, TResult> next) =>
            new SplittingReducer<TReduction>(Transducers, next);

        public IAsyncReducer<TReduction, TInput> Apply<TReduction>(IAsyncReducer<TReduction, TResult> next) =>
            new SplittingAsyncReducer<TReduction>(Transducers, next);
    }
}
