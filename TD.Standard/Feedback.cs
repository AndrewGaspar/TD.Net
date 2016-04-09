using System;
using System.Threading.Tasks;

namespace TD
{
    internal class Feedback<TInput> : ITransducer<TInput, TInput>
    {
        class Primary<TReduction> : IReducer<TReduction, TInput>
        {
            private readonly IReducer<TReduction, TInput> Next;

            public Primary(
                ITransducer<TInput, TInput> operation,
                IReducer<TReduction, TInput> next)
            {
                Next = operation.Apply(new Tail<TReduction>(this, next));
            }

            public Terminator<TReduction> Complete(TReduction reduction) => Next.Complete(reduction);

            public Terminator<TReduction> Invoke(TReduction reduction, TInput value) => 
                Next.Invoke(reduction, value);
        }

        class AsyncPrimary<TReduction> : IAsyncReducer<TReduction, TInput>
        {
            private readonly IAsyncReducer<TReduction, TInput> Next;

            public AsyncPrimary(
                ITransducer<TInput, TInput> operation,
                IAsyncReducer<TReduction, TInput> next)
            {
                Next = operation.Apply(new AsyncTail<TReduction>(this, next));
            }

            public Task<Terminator<TReduction>> CompleteAsync(TReduction reduction) => Next.CompleteAsync(reduction);

            public Task<Terminator<TReduction>> InvokeAsync(TReduction reduction, TInput value) =>
                Next.InvokeAsync(reduction, value);
        }
        
        class Tail<TReduction> : DefaultCompletionReducer<TReduction, TInput, TInput>
        {
            private readonly IReducer<TReduction, TInput> Loop;

            public Tail(
                IReducer<TReduction, TInput> primary,
                IReducer<TReduction, TInput> next) : base(next)
            {
                Loop = primary;
            }

            public override Terminator<TReduction> Invoke(TReduction reduction, TInput value)
            {
                var result = Next.Invoke(reduction, value);
                if (result.IsTerminated)
                {
                    return result;
                }

                return Loop.Invoke(reduction, value);
            }
        }
        
        class AsyncTail<TReduction> : DefaultCompletionAsyncReducer<TReduction, TInput, TInput>
        {
            private readonly IAsyncReducer<TReduction, TInput> Loop;

            public AsyncTail(
                IAsyncReducer<TReduction, TInput> primary,
                IAsyncReducer<TReduction, TInput> next) : base(next)
            {
                Loop = primary;
            }

            public override async Task<Terminator<TReduction>> InvokeAsync(TReduction reduction, TInput value)
            {
                var result = await Next.InvokeAsync(reduction, value).ConfigureAwait(false);
                if (result.IsTerminated)
                {
                    return result;
                }

                return await Loop.InvokeAsync(reduction, value).ConfigureAwait(false);
            }
        }

        private readonly ITransducer<TInput, TInput> Operation;

        public Feedback(ITransducer<TInput, TInput> operation)
        {
            Operation = operation;
        }

        public IReducer<TReduction, TInput> Apply<TReduction>(IReducer<TReduction, TInput> next) =>
            new Primary<TReduction>(Operation, next);

        public IAsyncReducer<TReduction, TInput> Apply<TReduction>(IAsyncReducer<TReduction, TInput> next) =>
            new AsyncPrimary<TReduction>(Operation, next);
    }
}
