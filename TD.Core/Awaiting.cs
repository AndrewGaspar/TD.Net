using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TD
{
    internal class Awaiting<TInput> : IAsyncTransducer<Task<TInput>, TInput>
    {
        private class Reducer<TReduction> : DefaultCompletionAsyncReducer<TReduction, Task<TInput>, TInput>
        {
            public Reducer(IAsyncReducer<TReduction, TInput> next) : base(next) { }

            public override async Task<Terminator<TReduction>> InvokeAsync(
                TReduction reduction, Task<TInput> value) => await Next.InvokeAsync(reduction, await value.ConfigureAwait(false)).ConfigureAwait(false);

        }

        public IAsyncReducer<TReduction, Task<TInput>> Apply<TReduction>(IAsyncReducer<TReduction, TInput> next) =>
            new Reducer<TReduction>(next);
    }
}
