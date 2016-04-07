using System;
using System.Threading.Tasks;

namespace TD
{
    /// <summary>
    /// Functions for creating Core transducers.
    /// </summary>
    public static class Core
    {
        /// <summary>
        /// Creates a transducer that performs a simple one-to-one mapping using the supplied
        /// mapping function.
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="map">The mapping function to use.</param>
        /// <returns>A transducer that maps input values to result values.</returns>
        public static ITransducer<TInput, TResult> Mapping<TInput, TResult>(Func<TInput, TResult> map) =>
            new Mapping<TInput, TResult>(map);

        /// <summary>
        /// Creates a new transducer that composes some transducer with a transducer that maps
        /// values given some mapping function.
        /// </summary>
        /// <typeparam name="TInput">The type of the input to the starting and resulting transducer.</typeparam>
        /// <typeparam name="TMedium">The type of the values commuted from the starting transducer and the mapping function.</typeparam>
        /// <typeparam name="TResult">The type of the values being mapped to.</typeparam>
        /// <param name="transducer">The transducer being mapped from.</param>
        /// <param name="map">The mapping function.</param>
        /// <returns>A transducer that maps values from the supplied transducer to values being produced from the new transducer.</returns>
        public static ITransducer<TInput, TResult> Mapping<TInput, TMedium, TResult>(
            this ITransducer<TInput, TMedium> transducer, Func<TMedium, TResult> map) => Compose(transducer, Mapping(map));

        public static IAsyncTransducer<TInput, TResult> AsyncMapping<TInput, TResult>(Func<TInput, Task<TResult>> map) =>
            new AsyncMapping<TInput, TResult>(map);

        /// <summary>
        /// Filters input values based on the supplied filtering functions.
        /// </summary>
        /// <typeparam name="TInput">The type of the values being filtered.</typeparam>
        /// <param name="test">The test. Returning true passes the input through.</param>
        /// <returns>A transducer that filters inputs based on the predicate.</returns>
        public static ITransducer<TInput, TInput> Filtering<TInput>(Predicate<TInput> test) =>
            new Filtering<TInput>(test);

        /// <summary>
        /// Produces a new transducer that composes the supplied transducer with a filtering transducer.
        /// </summary>
        /// <typeparam name="TInput">The type of the input to the starting and resulting transducer.</typeparam>
        /// <typeparam name="TResult">The type of the values produced from the starting and resulting transducer.</typeparam>
        /// <param name="transducer">The transducer being filtered from.</param>
        /// <param name="test">The test. Returning true passes the value through.</param>
        /// <returns>A transducer whose results are filtered from the supplied transducer.</returns>
        public static ITransducer<TInput, TResult> Filtering<TInput, TResult>(
            this ITransducer<TInput, TResult> transducer, Predicate<TResult> test) => Compose(transducer, Filtering(test));

        /// <summary>
        /// The identity transducer. All input is passed to the output.
        /// </summary>
        /// <typeparam name="TInput">The type of the transducers' input and result.</typeparam>
        /// <returns>A transducer that passes all input through.</returns>
        public static ITransducer<TInput, TInput> Passing<TInput>() => new Passing<TInput>();

        public static IAsyncReducer<TInput, TResult> AsAsync<TInput, TResult>(this IReducer<TInput, TResult> reducer) =>
            new AsyncConvertedReducer<TInput, TResult>(reducer);

        /// <summary>
        /// Composes two transducers together by attaching the output of the first to the input of the second.
        /// </summary>
        /// <typeparam name="A">The starting input.</typeparam>
        /// <typeparam name="B">b</typeparam>
        /// <typeparam name="C">The output type.</typeparam>
        /// <param name="first">The first transducer.</param>
        /// <param name="second">The second transducer.</param>
        /// <returns>A transducer composed from the input transducers.</returns>
        public static ITransducer<A, C> Compose<A, B, C>(
            this ITransducer<A, B> first,
            ITransducer<B, C> second) =>
                new Composing<A, B, C>(first, second);

        /// <summary>
        /// Composes two transducers together by attaching the output of the first to the input of the second.
        /// </summary>
        /// <typeparam name="A">The starting input.</typeparam>
        /// <typeparam name="B">b</typeparam>
        /// <typeparam name="C">c</typeparam>
        /// <typeparam name="D">The output type.</typeparam>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <param name="third">The third.</param>
        /// <returns>A transducer composed from the input transducers.</returns>
        public static ITransducer<A, D> Compose<A, B, C, D>(
            ITransducer<A, B> first,
            ITransducer<B, C> second,
            ITransducer<C, D> third) => Compose(first, second).Compose(third);

        /// <summary>
        /// Composes two transducers together by attaching the output of the first to the input of the second.
        /// </summary>
        /// <typeparam name="A">The input.</typeparam>
        /// <typeparam name="B">b</typeparam>
        /// <typeparam name="C">c</typeparam>
        /// <typeparam name="D">d</typeparam>
        /// <typeparam name="E">The result.</typeparam>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <param name="third">The third.</param>
        /// <param name="fourth">The fourth.</param>
        /// <returns>A transducer composed from the input transducers.</returns>
        public static ITransducer<A, E> Compose<A, B, C, D, E>(
            ITransducer<A, B> first,
            ITransducer<B, C> second,
            ITransducer<C, D> third,
            ITransducer<D, E> fourth) => Compose(first, second, third).Compose(fourth);

        /// <summary>
        /// Composes two transducers together by attaching the output of the first to the input of the second.
        /// </summary>
        /// <typeparam name="A">The input.</typeparam>
        /// <typeparam name="B">b</typeparam>
        /// <typeparam name="C">c</typeparam>
        /// <typeparam name="D">d</typeparam>
        /// <typeparam name="E">e</typeparam>
        /// <typeparam name="F">The result.</typeparam>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <param name="third">The third.</param>
        /// <param name="fourth">The fourth.</param>
        /// <param name="fifth">The fifth.</param>
        /// <returns>A transducer composed from the input transducers.</returns>
        public static ITransducer<A, F> Compose<A, B, C, D, E, F>(
            ITransducer<A, B> first,
            ITransducer<B, C> second,
            ITransducer<C, D> third,
            ITransducer<D, E> fourth,
            ITransducer<E, F> fifth) => Compose(first, second, third, fourth).Compose(fifth);

        /// <summary>
        /// Composes two transducers together by attaching the output of the first to the input of the second.
        /// </summary>
        /// <typeparam name="A">The input.</typeparam>
        /// <typeparam name="B">b</typeparam>
        /// <typeparam name="C">c</typeparam>
        /// <typeparam name="D">d</typeparam>
        /// <typeparam name="E">e</typeparam>
        /// <typeparam name="F">f</typeparam>
        /// <typeparam name="G">The result.</typeparam>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <param name="third">The third.</param>
        /// <param name="fourth">The fourth.</param>
        /// <param name="fifth">The fifth.</param>
        /// <param name="sixth">The sixth.</param>
        /// <returns>A transducer composed from the input transducers.</returns>
        public static ITransducer<A, G> Compose<A, B, C, D, E, F, G>(
            ITransducer<A, B> first,
            ITransducer<B, C> second,
            ITransducer<C, D> third,
            ITransducer<D, E> fourth,
            ITransducer<E, F> fifth,
            ITransducer<F, G> sixth) => Compose(first, second, third, fourth, fifth).Compose(sixth);
    }
}
