using System;
using System.Collections.Generic;
using static TD.Core;

namespace TD
{
    /// <summary>
    /// The standard library of Transducers. Provides a set of common, yet non-fundamental transducers.
    /// 
    /// For the fundamental transducers, see TD.Core;
    /// </summary>
    public static class Standard
    {
        /// <summary>
        /// Tries to parse values from input tokens.
        /// </summary>
        /// <typeparam name="TResult">The type of the parsed values.</typeparam>
        /// <returns>A transducer that produces values for parse-able values and null for non-parse-able values.</returns>
        public static ITransducer<string, TResult?> TryParsing<TResult>() where TResult : struct => 
            new TryParsingTransducer<TResult>();

        /// <summary>
        /// Produces a transducer that produces values parsed from the results of the supplied transducer.
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="transducer">The transducer.</param>
        /// <returns>A transducer that produces parsed values.</returns>
        public static ITransducer<TInput, TResult?> TryParsing<TInput, TResult>(
            this ITransducer<TInput, string> transducer) where TResult : struct =>
                transducer.Compose(TryParsing<TResult>());

        /// <summary>
        /// Creates a transducer that parses the input tokens or throws.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <returns>A transducer that produces results parsed from the input.</returns>
        public static ITransducer<string, TResult> Parsing<TResult>() where TResult : struct => 
            new ParsingTransducer<TResult>();

        /// <summary>
        /// Produces a transducer that parses the results of the supplied transducer.
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="transducer">The transducer.</param>
        /// <returns>A transducer whose results are parsed from the results of the supplied transducer.</returns>
        public static ITransducer<TInput, TResult> Parsing<TInput, TResult>(
            this ITransducer<TInput, string> transducer) where TResult : struct =>
            transducer.Compose(Parsing<TResult>());

        /// <summary>
        /// Takes the value from a null-able value. Does not guard by checking if it has a value.
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <returns>A transducer that takes null-able values and produces non-null values.</returns>
        public static ITransducer<TInput?, TInput> Dereferencing<TInput>() where TInput : struct => Mapping<TInput?, TInput>(x => x.Value);

        /// <summary>
        /// Filters through the non-null values.
        /// </summary>
        /// <typeparam name="TInput">The type of the base of the input.</typeparam>
        /// <returns>A transducer that dereferences non-null values.</returns>
        public static ITransducer<TInput?, TInput> FilteringNonNull<TInput>() where TInput : struct =>
            Filtering<TInput?>(x => x.HasValue).Compose(Dereferencing<TInput>());

        /// <summary>
        /// Produces a transducer that switches all input through the first matching switch.
        /// 
        /// If a switch's transducer produces a terminated result, input will no longer be passed to
        /// that switch's transducer, though its test will still stop the input from being applied
        /// to further switches.
        /// 
        /// A terminated result will only be produced when all of the switches produce terminating
        /// results.
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="switches">The switches.</param>
        /// <returns>A switching transducer that applies input to the transducer of the first matching switch.</returns>
        public static ITransducer<TInput, TResult> Switching<TInput, TResult>(params TransducerSwitch<TInput, TResult>[] switches) =>
            new Switching<TInput, TResult>(switches);

        /// <summary>
        /// Produces a transducer that switches all input through the first matching switch.
        /// 
        /// If a switch's transducer produces a terminated result, input will no longer be passed to
        /// that switch's transducer, though its test will still stop the input from being applied
        /// to further switches.
        /// 
        /// A terminated result will only be produced when all of the switches produce terminating
        /// results.
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="switches">The switches.</param>
        /// <returns>A switching transducer that applies input to the transducer of the first matching switch.</returns>
        public static ITransducer<TInput, TResult> Switching<TInput, TResult>(IList<TransducerSwitch<TInput, TResult>> switches) =>
            new Switching<TInput, TResult>(switches);

        /// <summary>
        /// A transducer that formats the input using the supplied format string.
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <param name="formatString">The format string.</param>
        /// <returns>A transducer that formats its input.</returns>
        public static ITransducer<TInput, string> Formatting<TInput>(string formatString) =>
            Mapping<TInput, string>(val => string.Format(formatString, val));

        /// <summary>
        /// A transducer that terminates the input immediately after the first matching input is passed through.
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <param name="test">The test.</param>
        /// <returns>A terminating transducer.</returns>
        public static ITransducer<TInput, TInput> Terminating<TInput>(Predicate<TInput> test) => 
            new Terminating<TInput>(test);

        /// <summary>
        /// A transducer that terminates the input after first value passes through.
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <returns>A terminating transducer</returns>
        public static ITransducer<TInput, TInput> Terminating<TInput>() => 
            Terminating<TInput>(_ => true);

        /// <summary>
        /// Relaxes input values from some derived type to a base type.
        /// </summary>
        /// <typeparam name="TDerived">The type of the input.</typeparam>
        /// <typeparam name="TBase">The type of the results.</typeparam>
        /// <returns>A transducer that relaxes the type of the supplied input.</returns>
        public static ITransducer<TDerived, TBase> Relaxing<TDerived, TBase>() where TDerived : TBase =>
            Mapping<TDerived, TBase>(a => a);

        /// <summary>
        /// Produces a transducer that relaxes the result type of the supplied transducer.
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <typeparam name="TDerived">The type produced by the original transducer.</typeparam>
        /// <typeparam name="TBase">The type produced by the relaxed transducer.</typeparam>
        /// <param name="transducer">The transducer.</param>
        /// <returns>A transducer that produces values of a relaxed type.</returns>
        public static ITransducer<TInput, TBase> Relaxing<TInput, TDerived, TBase>(this ITransducer<TInput, TDerived> transducer) 
            where TDerived : TBase => 
                transducer.Compose(Relaxing<TDerived, TBase>());

        /// <summary>
        /// Creates a transducer that erases the type of the input values.
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <returns>An erasing transducer.</returns>
        public static ITransducer<TInput, object> Erasing<TInput>() => Relaxing<TInput, object>();

        /// <summary>
        /// Produces a transducer that erases the type of the values produced by the supplied transducer.
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <typeparam name="TResult">The type of the results of the original transducer.</typeparam>
        /// <param name="transducer">The transducer.</param>
        /// <returns>An erasing transducer.</returns>
        public static ITransducer<TInput, object> Erasing<TInput, TResult>(
            this ITransducer<TInput, TResult> transducer) =>
                transducer.Compose(Relaxing<TResult, object>());

        /// <summary>
        /// Instantiates a transducer that passes values to a supplied 'success' transducer and catches exceptions in subsequent 
        /// reductions and passes the result to an exception handling transducer.
        /// </summary>
        /// <typeparam name="TInput">The input type</typeparam>
        /// <typeparam name="TResult">The result of types produced by both the success and exceptional transducers</typeparam>
        /// <typeparam name="TException">The type of exceptions to catch.</typeparam>
        /// <param name="success">The primary transducer that input is passed to.</param>
        /// <param name="exceptional">The transducer that handles exceptional cases.</param>
        /// <returns></returns>
        public static ITransducer<TInput, TResult> Catching<TInput, TResult, TException>(
            ITransducer<TInput, TResult> success,
            ITransducer<ExceptionalInput<TInput, TException>, TResult> exceptional)
                where TException : Exception =>
                    new Catching<TInput, TResult, TException>(success, exceptional);

        /// <summary>
        /// Casts a value from some base type to a derived type.
        /// </summary>
        /// <typeparam name="TBase">The base type</typeparam>
        /// <typeparam name="TDerived">The derived type</typeparam>
        /// <returns>A casting transducer.</returns>
        public static ITransducer<TBase, TDerived> Casting<TBase, TDerived>() where TDerived : TBase =>
            Mapping<TBase, TDerived>(x => (TDerived)x);

        /// <summary>
        /// Produces a transducer that composes the input transducer and restricts the type of the results from it.
        /// </summary>
        /// <typeparam name="TInput">The input to the new transducer</typeparam>
        /// <typeparam name="TBase">The original type of the transducer</typeparam>
        /// <typeparam name="TDerived">The restricted type of the transducer</typeparam>
        /// <param name="transducer">The original transducer</param>
        /// <returns>A new transducer that restricts the produced type</returns>
        public static ITransducer<TInput, TDerived> Casting<TInput, TBase, TDerived>(
            this ITransducer<TInput, TBase> transducer) where TDerived : TBase =>
                transducer.Compose(Casting<TBase, TDerived>());

        /// <summary>
        /// Any value applied to this transducer will be passed through the supplied transducer. The resulting
        /// value will first be applied to the given reducing function, then will be re-applied to the internal
        /// reducing function.
        /// </summary>
        /// <typeparam name="T">The type of the input and result.</typeparam>
        /// <param name="operation">The feedback operation</param>
        /// <returns>A feedback transducer</returns>
        public static ITransducer<T, T> Feedback<T>(ITransducer<T, T> operation) => new Feedback<T>(operation);

        /// <summary>
        /// Produces a feedback transducer composed with this transducer.
        /// </summary>
        /// <typeparam name="TInput">The input to the new transducer</typeparam>
        /// <typeparam name="TResult">The result and feedback type</typeparam>
        /// <param name="transducer">The original transducer</param>
        /// <param name="operation">The feedback operation.</param>
        /// <returns>A feedback transducer</returns>
        public static ITransducer<TInput, TResult> Feedback<TInput, TResult>(
            this ITransducer<TInput, TResult> transducer,
            ITransducer<TResult, TResult> operation) => transducer.Compose(Feedback(operation));

        /// <summary>
        /// Takes multiple values at a time. Each input pushes previous values out of the window of 
        /// values supplied. Does not produce a window until it's filled.
        /// </summary>
        /// <typeparam name="T">The input type</typeparam>
        /// <param name="windowSize">How large the window to fill is</param>
        /// <returns>A sliding transducer</returns>
        public static ITransducer<T, IList<T>> Sliding<T>(int windowSize) => new Sliding<T>(windowSize);

        /// <summary>
        /// Produces a transducer that takes multiple values from this transducer at a time.
        /// Each input pushes previous values out of the window of values supplied. Does not 
        /// produce a window until it's filled.
        /// </summary>
        /// <typeparam name="TInput">The input to the original and new transducer</typeparam>
        /// <typeparam name="TResult">The result of the original transducer</typeparam>
        /// <param name="transducer">The original transducer</param>
        /// <param name="windowSize">The size of the produced window</param>
        /// <returns>A sliding transducer</returns>
        public static ITransducer<TInput, IList<TResult>> Sliding<TInput, TResult>(
            this ITransducer<TInput, TResult> transducer,
            int windowSize) => transducer.Compose(Sliding<TResult>(windowSize));

        /// <summary>
        /// Creates a transducer that passes all input through all of the supplied transducers.
        /// </summary>
        /// <typeparam name="TInput">The input to each transducer</typeparam>
        /// <typeparam name="TResult">The result of each transducer</typeparam>
        /// <param name="transducers">The multiplexed transducers</param>
        /// <returns>A transducer that multiplexes the input</returns>
        public static ITransducer<TInput, TResult> Multiplexing<TInput, TResult>(
            IList<ITransducer<TInput, TResult>> transducers) => new Multiplexing<TInput, TResult>(transducers);

        /// <summary>
        /// Creates a transducer that passes all input through all of the supplied transducers.
        /// </summary>
        /// <typeparam name="TInput">The input to each transducer</typeparam>
        /// <typeparam name="TResult">The result of each transducer</typeparam>
        /// <param name="transducers">The multiplexed transducers</param>
        /// <returns>A transducer that multiplexes the input</returns>
        public static ITransducer<TInput, TResult> Multiplexing<TInput, TResult>(
            params ITransducer<TInput, TResult>[] transducers) => Multiplexing(transducers as IList<ITransducer<TInput, TResult>>);

        /// <summary>
        /// Creates a transducer that passes all input through all of the supplied transducers.
        /// </summary>
        /// <typeparam name="TInput">The input to the original transducer</typeparam>
        /// <typeparam name="TMultiInput">The input to the multi-transducers</typeparam>
        /// <typeparam name="TResult">The result of each transducer</typeparam>
        /// <param name="transducer">The original transducer</param>
        /// <param name="transducers">The multiplexed transducers</param>
        /// <returns>A transducer that multiplexes the original transducers' results</returns>
        public static ITransducer<TInput, TResult> Multiplexing<TInput, TMultiInput, TResult>(
            this ITransducer<TInput, TMultiInput> transducer,
            IList<ITransducer<TMultiInput, TResult>> transducers) => transducer.Compose(Multiplexing(transducers));


        /// <summary>
        /// Creates a transducer that passes all input through all of the supplied transducers.
        /// </summary>
        /// <typeparam name="TInput">The input to the original transducer</typeparam>
        /// <typeparam name="TMultiInput">The input to the multi-transducers</typeparam>
        /// <typeparam name="TResult">The result of each transducer</typeparam>
        /// <param name="transducer">The original transducer</param>
        /// <param name="transducers">The multiplexed transducers</param>
        /// <returns>A transducer that multiplexes the original transducers' results</returns>
        public static ITransducer<TInput, TResult> Multiplexing<TInput, TMultiInput, TResult>(
            this ITransducer<TInput, TMultiInput> transducer,
            params ITransducer<TMultiInput, TResult>[] transducers) => transducer.Compose(Multiplexing(transducers));
    }
}
