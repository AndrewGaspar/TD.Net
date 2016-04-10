using System;

namespace TD
{
    internal class AsyncComposing<TInput, TMedium, TResult> : IAsyncTransducer<TInput, TResult>
    {
        private readonly IAsyncTransducer<TInput, TMedium> Left;
        private readonly IAsyncTransducer<TMedium, TResult> Right;

        public AsyncComposing(
            IAsyncTransducer<TInput, TMedium> left,
            IAsyncTransducer<TMedium, TResult> right)
        {
            Left = left;
            Right = right;
        }

        public IAsyncReducer<TReduction, TInput> Apply<TReduction>(IAsyncReducer<TReduction, TResult> next) =>
            Left.Apply(Right.Apply(next));
    }

    internal class SyncComposing<TInput, TMedium, TResult> : ISyncTransducer<TInput, TResult>
    {
        private readonly ISyncTransducer<TInput, TMedium> Left;
        private readonly ISyncTransducer<TMedium, TResult> Right;

        public SyncComposing(
            ISyncTransducer<TInput, TMedium> left,
            ISyncTransducer<TMedium, TResult> right)
        {
            Left = left;
            Right = right;
        }

        public IReducer<TReduction, TInput> Apply<TReduction>(IReducer<TReduction, TResult> next) =>
            Left.Apply(Right.Apply(next));
    }

    internal class Composing<TInput, TMedium, TResult> : ITransducer<TInput, TResult>
    {
        private readonly ISyncTransducer<TInput, TResult> Sync;
        private readonly IAsyncTransducer<TInput, TResult> Async;

        public Composing(
            ITransducer<TInput, TMedium> left,
            ITransducer<TMedium, TResult> right)
        {
            Sync = left.SyncCompose(right);
            Async = left.AsyncCompose(right);
        }

        public IReducer<TReduction, TInput> Apply<TReduction>(IReducer<TReduction, TResult> next) =>
            Sync.Apply(next);

        public IAsyncReducer<TReduction, TInput> Apply<TReduction>(IAsyncReducer<TReduction, TResult> next) =>
            Async.Apply(next);
    }
}
