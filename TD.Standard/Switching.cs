using System;
using System.Collections.Generic;
using System.Linq;

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

    internal class Switching<From, To> : ITransducer<From, To>
    {
        class SplittingReducer<Reduction> : IReducer<Reduction, From>
        {
            class JoiningReducer : IReducer<Reduction, To>
            {
                private SplittingReducer<Reduction> splitter;
                private IReducer<Reduction, To> next;

                public JoiningReducer(
                    SplittingReducer<Reduction> splitter,
                    IReducer<Reduction, To> next)
                {
                    this.splitter = splitter;
                    this.next = next;
                }

                private bool CheckTermination(Terminator<Reduction> terminator)
                {
                    if (terminator.IsTerminated)
                    {
                        foreach (var reducer in splitter.reducers)
                        {
                            reducer.IsTerminated = true;
                        }

                        return true;
                    }

                    return false;
                }

                public Terminator<Reduction> Complete(Reduction reduction)
                {
                    var terminator = next.Complete(reduction);

                    var terminated = CheckTermination(terminator);

                    return Terminator.Reduction(terminator.Value, terminated: terminated);
                }

                public Terminator<Reduction> Invoke(Reduction reduction, To value)
                {
                    var terminator = next.Invoke(reduction, value);

                    var terminated = CheckTermination(terminator);

                    return Terminator.Reduction(terminator.Value, terminated: terminated);
                }
            }
            
            class ReducerOption
            {
                public ReducerOption(Predicate<From> test, IReducer<Reduction, From> reducer)
                {
                    Test = test;
                    IsTerminated = false;
                    Reducer = reducer;
                }

                public Predicate<From> Test { get; private set; }
                public bool IsTerminated { get; set; }
                public IReducer<Reduction, From> Reducer { get; set; }
            }

            private IList<ReducerOption> reducers;
            private IReducer<Reduction, To> next;

            public SplittingReducer(
                IList<TransducerSwitch<From, To>> transducers,
                IReducer<Reduction, To> next)
            {
                this.reducers = transducers
                    .Select(tSwitch =>
                        new ReducerOption(
                            tSwitch.Test, 
                            tSwitch.Transducer.Apply(new JoiningReducer(this, next))))
                    .ToList();
                this.next = next;
            }

            private ReducerOption GetMatchingReducer(From value) =>
                reducers.First(reducer => reducer.Test(value));

            public Terminator<Reduction> Invoke(Reduction reduction, From value)
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

                return Terminator.Reduction(reduction, terminated: reducers.All(red => red.IsTerminated));
            }

            public Terminator<Reduction> Complete(Reduction reduction) =>
                reducers.Where(reducer => !reducer.IsTerminated)
                        .Aggregate(Terminator.Reduction(reduction), (term, reducer) =>
                            reducer.Reducer.Complete(reduction));
        }


        public IList<TransducerSwitch<From, To>> Transducers { get; private set; }

        public Switching(IList<TransducerSwitch<From, To>> transducers)
        {
            Transducers = transducers;
        }

        public IReducer<Reduction, From> Apply<Reduction>(IReducer<Reduction, To> next)
        {
            return new SplittingReducer<Reduction>(Transducers, next);
        }
    }
}
