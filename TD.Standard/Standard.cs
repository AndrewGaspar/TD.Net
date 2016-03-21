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
        public static ITransducer<string, TResult?> TryParsing<TResult>() where TResult : struct => new TryParsing<TResult>();

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
        public static ITransducer<TInput, TInput> Terminating<TInput>(Predicate<TInput> test) => new Terminating<TInput>(test);

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
        public static ITransducer<TInput, object> Erasing<TInput, TResult>(this ITransducer<TInput, TResult> transducer) =>
            transducer.Compose(Relaxing<TResult, object>());
    }
}
