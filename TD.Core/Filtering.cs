using System;
using System.Threading.Tasks;
using static TD.Terminator;

namespace TD
{
    internal class Filtering<TInput> : ITransducer<TInput, TInput>
    {
        private class Reducer<TReduction> : DefaultCompletionReducer<TReduction, TInput, TInput>
        {
            private readonly Predicate<TInput> Test;

            public Reducer(
                Predicate<TInput> test,
                IReducer<TReduction, TInput> reducer) : base(reducer)
            {
                Test = test;
            }

            public override Terminator<TReduction> Invoke(TReduction reduction, TInput value) =>
                Test(value) ? Next.Invoke(reduction, value) : Reduction(reduction);
        }

        private class AsyncReducer<TReduction> : DefaultCompletionAsyncReducer<TReduction, TInput, TInput>
        {
            private readonly Predicate<TInput> Test;

            public AsyncReducer(
                Predicate<TInput> test,
                IAsyncReducer<TReduction, TInput> next) : base(next)
            {
                Test = test;
            }

            public override Task<Terminator<TReduction>> InvokeAsync(TReduction reduction, TInput value) =>
                Test(value) ? Next.InvokeAsync(reduction, value) : Task.FromResult(Reduction(reduction));
        }

        public Predicate<TInput> Test { get; private set; }

        public Filtering(Predicate<TInput> test)
        {
            Test = test;
        }

        public IReducer<TReduction, TInput> Apply<TReduction>(IReducer<TReduction, TInput> reducer) =>
            new Reducer<TReduction>(Test, reducer);

        public IAsyncReducer<TReduction, TInput> Apply<TReduction>(IAsyncReducer<TReduction, TInput> next) =>
            new AsyncReducer<TReduction>(Test, next);
    }
}
