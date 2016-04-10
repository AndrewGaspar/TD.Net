using System.IO;
using static TD.Standard;

namespace TD
{
    /// <summary>
    /// Helpers for using TextReader and TextWriter classes with transducers.
    /// </summary>
    public static class TextIO
    {
        /// <summary>
        /// Pulls values from the supplied TextReader and passes them into the supplied reducer.
        /// </summary>
        /// <typeparam name="TReduction">The type of the reduction.</typeparam>
        /// <param name="reader">The reader.</param>
        /// <param name="reduction">The reduction.</param>
        /// <param name="reducer">The reducer.</param>
        /// <returns>A wrapped reduction.</returns>
        public static Terminator<TReduction> Reduce<TReduction>(
            this TextReader reader,
            TReduction reduction,
            IReducer<TReduction, string> reducer)
        {
            var terminator = Terminator.Reduction(reduction);

            for (var value = reader.ReadLine(); value != null; value = reader.ReadLine())
            {
                terminator = reducer.Invoke(terminator.Value, value);
                if(terminator.IsTerminated)
                {
                    return terminator;
                }
            }

            return reducer.Complete(reduction);
        }

        /// <summary>
        /// Passes results from some TextReader through the supplied transducer to some TextWriter.
        /// </summary>
        /// <typeparam name="TResult">The type the transducer produces.</typeparam>
        /// <param name="input">The input.</param>
        /// <param name="output">The output.</param>
        /// <param name="transducer">The transducer.</param>
        public static void Transduce<TResult>(
            this TextReader input,
            TextWriter output,
            ITransducer<string, TResult> transducer) =>
            input.Reduce(output, transducer.Apply(WriteReducer<TResult>()));

        /// <summary>
        /// Returns a reducer that writes the input to a TextWriter.
        /// </summary>
        /// <returns>A reducer.</returns>
        public static IReducer<TextWriter, object> WriteReducer() =>
            Reducer.Make<TextWriter, object>((writer, input) =>
            {
                writer.Write(input);
                return writer;
            });

        /// <summary>
        /// Returns a reducer that writes the input to a TextWriter.
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <returns>A reducer.</returns>
        public static IReducer<TextWriter, TInput> WriteReducer<TInput>() => 
            Erasing<TInput>().Apply(WriteReducer());

        public static IReducer<TextWriter, TInput> WriteReducer<TInput, TResult>(
            this ITransducer<TInput, TResult> transducer) => transducer.Apply(WriteReducer<TResult>());

        /// <summary>
        /// Returns a reducer that writes the input to a TextWriter.
        /// </summary>
        /// <returns>A reducer.</returns>
        public static IAsyncReducer<TextWriter, string> AsyncWriteReducer() =>
            Reducer.AsyncMake<TextWriter, string>(async (writer, input) =>
            {
                await writer.WriteAsync(input).ConfigureAwait(false);
                return writer;
            });

        public static IAsyncReducer<TextWriter, TInput> AsyncWriteReducer<TInput>(
            this IAsyncTransducer<TInput, string> transducer) => transducer.Apply(AsyncWriteReducer());

        /// <summary>
        /// Returns a reducer that writes the input to a TextWriter line-by-line.
        /// </summary>
        /// <returns>A reducer</returns>
        public static IReducer<TextWriter, object> WriteLineReducer() =>
            Reducer.Make<TextWriter, object>((writer, input) =>
            {
                writer.WriteLine(input);
                return writer;
            });

        /// <summary>
        /// Returns a reducer that writes the input to a TextWriter line-by-line.
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <returns>A reducer.</returns>
        public static IReducer<TextWriter, TInput> WriteLineReducer<TInput>() =>
            Erasing<TInput>().Apply(WriteLineReducer());

        public static IReducer<TextWriter, TInput> WriteLineReducer<TInput, TResult>(
            this ITransducer<TInput, TResult> transducer) => transducer.Apply(WriteLineReducer<TResult>());

        public static IAsyncReducer<TextWriter, string> AsyncWriteLineReducer() =>
            Reducer.AsyncMake<TextWriter, string>(async (writer, input) =>
            {
                await writer.WriteLineAsync(input).ConfigureAwait(false);
                return writer;
            });

        public static IAsyncReducer<TextWriter, TInput> AsyncWriteLineReducer<TInput>(
            this IAsyncTransducer<TInput, string> transducer) => transducer.Apply(AsyncWriteLineReducer());
    }
}
