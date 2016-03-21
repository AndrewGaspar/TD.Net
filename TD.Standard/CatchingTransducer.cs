using System;

namespace TD
{
    public struct ExceptionalInput<TInput, TException> where TException : Exception
    {
        public readonly TInput Input;
        public readonly TException Exception;

        internal ExceptionalInput(TInput input, TException exception)
        {
            Input = input;
            Exception = exception;
        }
    }

    internal class CatchingTransducer<TInput, TResult, TException> : ITransducer<TInput, TResult>
        where TException : Exception
    {
        class Reducer<TReduction> : DefaultCompletionReducer<TReduction, TInput, TResult>
        {
            private readonly IReducer<TReduction, TInput> success;
            private readonly IReducer<TReduction, ExceptionalInput<TInput, TException>> exceptional;

            public Reducer(
                CatchingTransducer<TInput, TResult, TException> transducer,
                IReducer<TReduction, TResult> next) : base(next)
            {
                success = transducer.success.Apply(next);
                exceptional = transducer.exceptional.Apply(next);
            }

            public override Terminator<TReduction> Invoke(TReduction reduction, TInput value)
            {
                try
                {
                    return success.Invoke(reduction, value);
                }
                catch (TException exception)
                {
                    return exceptional.Invoke(
                        reduction,
                        new ExceptionalInput<TInput, TException>(value, exception));
                }
            }
        }

        private readonly ITransducer<TInput, TResult> success;
        private readonly ITransducer<ExceptionalInput<TInput, TException>, TResult> exceptional;

        public CatchingTransducer(
            ITransducer<TInput, TResult> successTransducer,
            ITransducer<ExceptionalInput<TInput, TException>, TResult> exceptionalTransducer)
        {
            success = successTransducer;
            exceptional = exceptionalTransducer;
        }

        public IReducer<Reduction, TInput> Apply<Reduction>(IReducer<Reduction, TResult> reducer) =>
            new Reducer<Reduction>(this, reducer);
    }
}
