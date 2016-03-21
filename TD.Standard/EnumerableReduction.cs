using System.Collections.Generic;
using System.Linq;

namespace TD
{
    /// <summary>
    /// Helpers for using IEnumerable with transducers.
    /// </summary>
    public static class Enumerable
    {
        /// <summary>
        /// A reducer for adding elements to an IList.
        /// </summary>
        /// <typeparam name="TResult">The type of the values in the IList.</typeparam>
        /// <returns>An IReducer that pushes values to an IList.</returns>
        public static IReducer<IList<TResult>, TResult> ListReducer<TResult>() =>
            Reducer.Make<IList<TResult>, TResult>((list, val) =>
            {
                list.Add(val);
                return list;
            });

        /// <summary>
        /// Passes elements from an IEnumerable into the supplied reducer.
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <typeparam name="TReduction">The type of the reduction.</typeparam>
        /// <param name="input">The input.</param>
        /// <param name="reduction">The reduction.</param>
        /// <param name="reducer">The reducer.</param>
        /// <returns>A terminated reduction.</returns>
        public static Terminator<TReduction> Reduce<TInput, TReduction>(
            this IEnumerable<TInput> input,
            TReduction reduction,
            IReducer<TReduction, TInput> reducer)
        {
            var terminator = Terminator.Reduction(reduction);

            foreach (var value in input)
            {
                terminator = reducer.Invoke(terminator.Value, value);
                
                if(terminator.IsTerminated)
                {
                    return terminator;
                }
            }

            return reducer.Complete(terminator.Value);
        }

        /// <summary>
        /// Takes the input enumerable and produces a new enumerable by passing
        /// each value through the supplied transducer one-by-one and yielding
        /// the results.
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="input">The input.</param>
        /// <param name="transducer">The transducer.</param>
        /// <returns>
        /// An enumerable that is a sequence of values after the input is applied to 
        /// the supplied transducer.
        /// </returns>
        public static IEnumerable<TResult> Transduce<TInput, TResult>(
            this IEnumerable<TInput> input,
            ITransducer<TInput, TResult> transducer)
        {
            var reducer = transducer.Apply(Enumerable.ListReducer<TResult>());
            var list = new List<TResult>();

            foreach (var value in input)
            {
                var reduction = reducer.Invoke(list, value);

                foreach (var result in reduction.Value)
                {
                    yield return result;
                }

                if (reduction.IsTerminated)
                {
                    yield break;
                }

                list.Clear();
            }

            var completionReduction = reducer.Complete(list);
            foreach (var result in completionReduction.Value)
            {
                yield return result;
            }
        }

        /// <summary>
        /// Passes the values from the enumerable through the supplied transducer and
        /// collects the results into a list.
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="input">The input.</param>
        /// <param name="transducer">The transducer.</param>
        /// <returns>A list of the computed results.</returns>
        public static IList<TResult> Collect<TInput, TResult>(
            this IEnumerable<TInput> input,
            ITransducer<TInput, TResult> transducer) =>
            input.Reduce(new List<TResult>(), transducer.Apply(ListReducer<TResult>())).Value;
    }
}
