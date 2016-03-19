using System;
using System.Collections.Generic;
using System.Linq;

namespace TD
{
    public static class TransducerSwitch
    {
        public static TransducerSwitch<From, To> Create<From, To>(Predicate<From> test, ITransducer<From, To> transducer) =>
            new TransducerSwitch<From, To>(test, transducer);

        public static TransducerSwitch<From, To> Default<From, To>(ITransducer<From, To> transducer) =>
            Create(_ => true, transducer);
    }

    public class TransducerSwitch<From, To>
    {
        public TransducerSwitch(Predicate<From> test, ITransducer<From, To> transducer)
        {
            Test = test;
            Transducer = transducer;
        }

        public Predicate<From> Test { get; private set; }
        public ITransducer<From, To> Transducer { get; private set; }
    }

    public class Switching<From, To> : ITransducer<From, To>
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
                    if (terminator.Terminated)
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

                    if (terminator.Terminated)
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
