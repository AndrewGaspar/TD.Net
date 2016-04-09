using System;
using System.Threading.Tasks;

namespace TD
{
    internal class Terminating<T> : ITransducer<T, T>
    {
        class Reducer<Reduction> : DefaultCompletionReducer<Reduction, T, T>
        {
            private Predicate<T> Test;

            public Reducer(Predicate<T> test, IReducer<Reduction, T> next) : base(next)
            {
                Test = test;
            }

            public override Terminator<Reduction> Invoke(Reduction reduction, T value)
            {
                var terminator = Next.Invoke(reduction, value);

                if (Test(value))
                {
                    return Terminator.Termination(terminator.Value);
                }

                return terminator;
            }
        }

        class AsyncReducer<Reduction> : DefaultCompletionAsyncReducer<Reduction, T, T>
        {
            private Predicate<T> Test;

            public AsyncReducer(Predicate<T> test, IAsyncReducer<Reduction, T> next) : base(next)
            {
                Test = test;
            }

            public override async Task<Terminator<Reduction>> InvokeAsync(Reduction reduction, T value)
            {
                var terminator = await Next.InvokeAsync(reduction, value);

                if (Test(value))
                {
                    return Terminator.Termination(terminator.Value);
                }

                return terminator;
            }
        }

        public Predicate<T> Test { get; private set; }

        public Terminating(Predicate<T> test)
        {
            Test = test;
        }

        public IReducer<TReduction, T> Apply<TReduction>(IReducer<TReduction, T> next) =>
            new Reducer<TReduction>(Test, next);

        public IAsyncReducer<TReduction, T> Apply<TReduction>(IAsyncReducer<TReduction, T> next) =>
            new AsyncReducer<TReduction>(Test, next);
    }
}
