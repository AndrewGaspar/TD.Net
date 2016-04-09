using System;
using System.Threading.Tasks;

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

    internal class Catching<TInput, TResult, TException> : ITransducer<TInput, TResult>
        where TException : Exception
    {
        class Reducer<TReduction> : DefaultCompletionReducer<TReduction, TInput, TResult>
        {
            private readonly IReducer<TReduction, TInput> success;
            private readonly IReducer<TReduction, ExceptionalInput<TInput, TException>> exceptional;

            public Reducer(
                Catching<TInput, TResult, TException> transducer,
                IReducer<TReduction, TResult> next) : base(next)
            {
                success = transducer.Success.Apply(next);
                exceptional = transducer.Exceptional.Apply(next);
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

        class AsyncReducer<TReduction> : DefaultCompletionAsyncReducer<TReduction, TInput, TResult>
        {
            private readonly IAsyncReducer<TReduction, TInput> success;
            private readonly IAsyncReducer<TReduction, ExceptionalInput<TInput, TException>> exceptional;

            public AsyncReducer(
                Catching<TInput, TResult, TException> transducer,
                IAsyncReducer<TReduction, TResult> next) : base(next)
            {
                success = transducer.Success.Apply(next);
                exceptional = transducer.Exceptional.Apply(next);
            }

            public override async Task<Terminator<TReduction>> InvokeAsync(TReduction reduction, TInput value)
            {
                try
                {
                    return await success.InvokeAsync(reduction, value);
                }
                catch (TException exception)
                {
                    return await exceptional.InvokeAsync(
                        reduction,
                        new ExceptionalInput<TInput, TException>(value, exception));
                }
            }
        }

        private readonly ITransducer<TInput, TResult> Success;
        private readonly ITransducer<ExceptionalInput<TInput, TException>, TResult> Exceptional;

        public Catching(
            ITransducer<TInput, TResult> successTransducer,
            ITransducer<ExceptionalInput<TInput, TException>, TResult> exceptionalTransducer)
        {
            Success = successTransducer;
            Exceptional = exceptionalTransducer;
        }

        public IReducer<TReduction, TInput> Apply<TReduction>(IReducer<TReduction, TResult> next) =>
            new Reducer<TReduction>(this, next);

        public IAsyncReducer<TReduction, TInput> Apply<TReduction>(IAsyncReducer<TReduction, TResult> next) =>
            new AsyncReducer<TReduction>(this, next);
    }
}
