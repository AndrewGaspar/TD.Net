using System;
using System.Threading.Tasks;

namespace TD
{
    /// <summary>
    /// IReducer represents a computation that takes some value and returns a reduced aggregate value.
    /// 
    /// An IReducer can be stateful. All returned reductions are wrapped by a <see cref="Terminator">Terminator</see> type that signifies 
    /// whether the IReducer can accept any further input. If <see cref="Terminator{Reduction}.IsTerminated">IsTerminated</see> evaluates
    /// to true, no further input should be applied to the IReducer, either via its Invoke() or Complete()
    /// method.
    /// 
    /// When the input source terminates and no further input is available to apply to the reducer, Complete()
    /// should be called with the current reduction value to flush any pending values in the reducing computation.
    /// </summary>
    /// <typeparam name="TReduction">The type of reduction that the reducer acts on.</typeparam>
    /// <typeparam name="TInput">The type of input this reducer can accept.</typeparam>
    public interface IReducer<TReduction, TInput>
    {
        /// <summary>
        /// Supplies new input to the IReducer.
        /// </summary>
        /// <param name="reduction">An initial/intermediate reduction.</param>
        /// <param name="value">The value to apply to the IReducer.</param>
        /// <returns>A wrapped reduction that indicates whether input should terminate.</returns>
        Terminator<TReduction> Invoke(TReduction reduction, TInput value);

        /// <summary>
        /// Indicates that input has been exhausted and that the reducer should flush any pending values.
        /// </summary>
        /// <param name="reduction">The current reduction value.</param>
        /// <returns>A wrapped reduction.</returns>
        Terminator<TReduction> Complete(TReduction reduction);
    }

    public interface IAsyncReducer<TReduction, TInput>
    {
        Task<Terminator<TReduction>> InvokeAsync(TReduction reduction, TInput value);

        Task<Terminator<TReduction>> CompleteAsync(TReduction reduction);
    }

    internal class AsyncConvertedReducer<TReduction, TInput> : IAsyncReducer<TReduction, TInput>
    {
        private readonly IReducer<TReduction, TInput> Reducer;

        public AsyncConvertedReducer(IReducer<TReduction, TInput> reducer)
        {
            Reducer = reducer;
        }

        public Task<Terminator<TReduction>> CompleteAsync(TReduction reduction) =>
            Task.FromResult(Reducer.Complete(reduction));

        public Task<Terminator<TReduction>> InvokeAsync(TReduction reduction, TInput value) =>
            Task.FromResult(Reducer.Invoke(reduction, value));
    }

    /// <summary>
    /// A helper for implementing IReducer that stores a reference to the next IReducer in the chain.
    /// </summary>
    /// <typeparam name="TReduction">The type of reduction that the IReducer acts on.</typeparam>
    /// <typeparam name="TInput">The type of values this reducer takes as input.</typeparam>
    /// <typeparam name="TForward">The type of values the next reducer in the chain takes as input.</typeparam>
    public abstract class BaseReducer<TReduction, TInput, TForward> : IReducer<TReduction, TInput>
    {
        /// <summary>
        /// The Next IReducer in the chain. This Reducer wraps this reducer and produces a new one.
        /// </summary>
        protected readonly IReducer<TReduction, TForward> Next;

        /// <summary>
        /// Constructs a new reducer.
        /// </summary>
        /// <param name="next">The IReducer that this reducer forwards transformed input to.</param>
        protected BaseReducer(IReducer<TReduction, TForward> next)
        {
            Next = next;
        }

        /// <summary>
        /// Supplies new input to the IReducer.
        /// </summary>
        /// <param name="reduction">An initial/intermediate reduction.</param>
        /// <param name="value">The value to apply to the IReducer.</param>
        /// <returns>
        /// A wrapped reduction that indicates whether input should terminate.
        /// </returns>
        public abstract Terminator<TReduction> Invoke(TReduction reduction, TInput value);

        /// <summary>
        /// Called when input is exhausted.
        /// </summary>
        /// <param name="reduction">The current reduction.</param>
        /// <returns>A wrapped reduction.</returns>
        public abstract Terminator<TReduction> Complete(TReduction reduction);
    }

    /// <summary>
    /// A helper class for creating Reducers that have no special Completion behavior.
    /// 
    /// Forwards all Complete calls to the Next reducer.
    /// </summary>
    /// <typeparam name="TReduction">The type of the reduction.</typeparam>
    /// <typeparam name="TInput">The type that this reducer takes as input.</typeparam>
    /// <typeparam name="TForward">
    /// The type that <see cref="BaseReducer{Reduction, TInput, TForward}.Next"/>
    /// takes as input.
    /// </typeparam>
    /// <seealso cref="TD.BaseReducer{Reduction, TInput, TForward}" />
    public abstract class DefaultCompletionReducer<TReduction, TInput, TForward> : BaseReducer<TReduction, TInput, TForward>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultCompletionReducer{Reduction, From, To}"/> class.
        /// </summary>
        /// <param name="next">The IReducer that this reducer forwards transformed input to.</param>
        protected DefaultCompletionReducer(IReducer<TReduction, TForward> next) : base(next)
        {
        }
        
        /// <summary>
        /// Called when input is exhausted.
        /// </summary>
        /// <param name="reduction">The current reduction.</param>
        /// <returns>
        /// A wrapped reduction.
        /// </returns>
        public override Terminator<TReduction> Complete(TReduction reduction) => Next.Complete(reduction);
    }
}
