using System;
using System.Threading.Tasks;

namespace TD
{
    internal class Mapping<From, To> : ITransducer<From, To>
    {
        private class Reducer<Reduction> : DefaultCompletionReducer<Reduction, From, To>
        {
            private readonly Func<From, To> MappingFunction;

            public Reducer(
                Func<From, To> mappingFunction,
                IReducer<Reduction, To> reducer) : base(reducer)
            {
                MappingFunction = mappingFunction;
            }

            public override Terminator<Reduction> Invoke(Reduction reduction, From value) =>
                Next.Invoke(reduction, MappingFunction(value));
        }

        public Func<From, To> MappingFunction { get; private set; }

        public Mapping(Func<From, To> func)
        {
            MappingFunction = func;
        }

        public IReducer<Reduction, From> Apply<Reduction>(IReducer<Reduction, To> reducer) =>
            new Reducer<Reduction>(MappingFunction, reducer);
    }

    internal class AsyncMapping<From, To> : IAsyncTransducer<From, To>
    {
        private class Reducer<TReduction> : IAsyncReducer<TReduction, From>
        {
            private readonly Func<From, Task<To>> MappingFunction;
            private readonly IAsyncReducer<TReduction, To> Next;

            public Reducer(
                Func<From, Task<To>> mappingFunction,
                IAsyncReducer<TReduction, To> reducer)
            {
                MappingFunction = mappingFunction;
                Next = reducer;
            }

            public Task<Terminator<TReduction>> CompleteAsync(TReduction reduction) =>
                Next.CompleteAsync(reduction);

            public async Task<Terminator<TReduction>> InvokeAsync(TReduction reduction, From value) =>
                await Next.InvokeAsync(reduction, await MappingFunction(value).ConfigureAwait(false)).ConfigureAwait(false);
        }

        public Func<From, Task<To>> MappingFunction { get; private set; }

        public AsyncMapping(Func<From, Task<To>> func)
        {
            MappingFunction = func;
        }

        public IAsyncReducer<Reduction, From> Apply<Reduction>(IAsyncReducer<Reduction, To> reducer) =>
            new Reducer<Reduction>(MappingFunction, reducer);
    }
}
