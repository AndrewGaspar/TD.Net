using System;

namespace TD
{
    internal class Composing<TInput, TMedium, TResult> : ITransducer<TInput, TResult>
    {
        public ITransducer<TInput, TMedium> Left { get; private set; }
        public ITransducer<TMedium, TResult> Right { get; private set; }

        public Composing(
            ITransducer<TInput, TMedium> left,
            ITransducer<TMedium, TResult> right)
        {
            Left = left;
            Right = right;
        }

        public IReducer<TReduction, TInput> Apply<TReduction>(IReducer<TReduction, TResult> next) =>
            Left.Apply(Right.Apply(next));

        public IAsyncReducer<TReduction, TInput> Apply<TReduction>(IAsyncReducer<TReduction, TResult> next) =>
            Left.Apply(Right.Apply(next));
    }
}
