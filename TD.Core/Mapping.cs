using System;
using System.Threading.Tasks;

namespace TD
{
    internal class Mapping<TInput, TResult> : ITransducer<TInput, TResult>
    {
        private class Reducer<Reduction> : DefaultCompletionReducer<Reduction, TInput, TResult>
        {
            private readonly Func<TInput, TResult> MappingFunction;

            public Reducer(
                Func<TInput, TResult> mappingFunction,
                IReducer<Reduction, TResult> reducer) : base(reducer)
            {
                MappingFunction = mappingFunction;
            }

            public override Terminator<Reduction> Invoke(Reduction reduction, TInput value) =>
                Next.Invoke(reduction, MappingFunction(value));
        }

        private class AsyncReducer<TReduction> : DefaultCompletionAsyncReducer<TReduction, TInput, TResult>
        {
            private readonly Func<TInput, TResult> MappingFunction;

            public AsyncReducer(
                Func<TInput, TResult> mappingFunction,
                IAsyncReducer<TReduction, TResult> reducer) : base(reducer)
            {
                MappingFunction = mappingFunction;
            }

            public override Task<Terminator<TReduction>> InvokeAsync(TReduction reduction, TInput value) =>
                Next.InvokeAsync(reduction, MappingFunction(value));
        }

        private readonly Func<TInput, TResult> MappingFunction;

        public Mapping(Func<TInput, TResult> func)
        {
            MappingFunction = func;
        }

        public IReducer<TReduction, TInput> Apply<TReduction>(IReducer<TReduction, TResult> reducer) =>
            new Reducer<TReduction>(MappingFunction, reducer);

        public IAsyncReducer<TReduction, TInput> Apply<TReduction>(IAsyncReducer<TReduction, TResult> next) =>
            new AsyncReducer<TReduction>(MappingFunction, next);
    }
}
