using System;

namespace TD
{
    internal class Feedback<T> : ITransducer<T, T>
    {
        class Primary<TReduction> : IReducer<TReduction, T>
        {
            private readonly IReducer<TReduction, T> Next;

            public Primary(
                ITransducer<T, T> operation,
                IReducer<TReduction, T> next)
            {
                Next = operation.Apply(new Tail<TReduction>(this, next));
            }

            public Terminator<TReduction> Complete(TReduction reduction) => Next.Complete(reduction);

            public Terminator<TReduction> Invoke(TReduction reduction, T value) => 
                Next.Invoke(reduction, value);
        }

        class Tail<TReduction> : DefaultCompletionReducer<TReduction, T, T>
        {
            private readonly IReducer<TReduction, T> Loop;

            public Tail(
                IReducer<TReduction, T> primary,
                IReducer<TReduction, T> next) : base(next)
            {
                Loop = primary;
            }

            public override Terminator<TReduction> Invoke(TReduction reduction, T value)
            {
                var result = Next.Invoke(reduction, value);
                if (result.IsTerminated)
                {
                    return result;
                }

                return Loop.Invoke(reduction, value);
            }
        }

        private readonly ITransducer<T, T> Operation;

        public Feedback(ITransducer<T, T> operation)
        {
            Operation = operation;
        }

        public IReducer<TReduction, T> Apply<TReduction>(IReducer<TReduction, T> next)
        {
            return new Primary<TReduction>(Operation, next);
        }
    }
}
