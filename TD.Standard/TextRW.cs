using System.IO;

namespace TD
{
    public static class TextRW
    {
        public static Reduction Reduce<Reduction>(
            this TextReader reader,
            Reduction reduction,
            IReducer<Reduction, string> reducer)
        {
            for (var value = reader.ReadLine(); value != null; value = reader.ReadLine())
            {
                var terminator = reducer.Invoke(reduction, value);
                if(terminator.Terminated)
                {
                    return terminator.Value;
                }
            }

            return reducer.Complete(reduction).Value;
        }

        public static IReducer<TextWriter, Input> WriteReducer<Input>()
        {
            return Reducer.Make<TextWriter, Input>((writer, input) =>
            {
                writer.Write(input.ToString());
                return writer;
            });
        }

        public static IReducer<TextWriter, Input> WriteLineReducer<Input>()
        {
            return Reducer.Make<TextWriter, Input>((writer, input) =>
            {
                writer.WriteLine(input.ToString());
                return writer;
            });
        }
    }
}
