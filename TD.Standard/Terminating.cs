using System;

namespace TD
{
    internal class Terminating<T> : ITransducer<T, T>
    {
        class Reducer<Reduction> : IReducer<Reduction, T>
        {
            private Predicate<T> test;
            private IReducer<Reduction, T> next;

            public Reducer(Predicate<T> test, IReducer<Reduction, T> next)
            {
                this.test = test;
                this.next = next;
            }

            public Terminator<Reduction> Invoke(Reduction reduction, T value)
            {
                var terminator = next.Invoke(reduction, value);

                if (test(value))
                {
                    return Terminator.Termination(terminator.Value);
                }

                return terminator;
            }

            public Terminator<Reduction> Complete(Reduction reduction) => next.Complete(reduction);
        }

        public Predicate<T> Test { get; private set; }

        public Terminating(Predicate<T> test)
        {
            Test = test;
        }

        public IReducer<Reduction, T> Apply<Reduction>(IReducer<Reduction, T> reducer)
        {
            return new Reducer<Reduction>(Test, reducer);
        }
    }
}
