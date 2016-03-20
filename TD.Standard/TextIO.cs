using System.IO;
using static TD.Standard;

namespace TD
{
    public static class TextIO
    {
        public static Terminator<Reduction> Reduce<Reduction>(
            this TextReader reader,
            Reduction reduction,
            IReducer<Reduction, string> reducer)
        {
            var terminator = Terminator.Reduction(reduction);

            for (var value = reader.ReadLine(); value != null; value = reader.ReadLine())
            {
                terminator = reducer.Invoke(terminator.Value, value);
                if(terminator.Terminated)
                {
                    return terminator;
                }
            }

            return reducer.Complete(reduction);
        }

        public static void Transduce<T>(
            this TextReader input,
            TextWriter output,
            ITransducer<string, T> transducer) =>
            input.Reduce(output, transducer.Apply(WriteReducer<T>()));

        public static IReducer<TextWriter, object> WriteReducer() =>
            Reducer.Make<TextWriter, object>((writer, input) =>
            {
                writer.Write(input);
                return writer;
            });

        public static IReducer<TextWriter, T> WriteReducer<T>() => Relaxing<T>().Apply(WriteReducer());

        public static IReducer<TextWriter, object> WriteLineReducer() =>
            Reducer.Make<TextWriter, object>((writer, input) =>
            {
                writer.WriteLine(input);
                return writer;
            });

        public static IReducer<TextWriter, Input> WriteLineReducer<Input>() =>
            Relaxing<Input>().Apply(WriteLineReducer());
    }
}
