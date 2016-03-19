using System.Collections.Generic;

namespace TD
{
    public static class Collection
    {
        public static IReducer<IList<Result>, Result> ListReducer<Result>() =>
            Reducer.Make<IList<Result>, Result>((list, val) =>
            {
                list.Add(val);
                return list;
            });
    }

    public static class Extensions
    {
        public static Terminator<Reduction> Reduce<Input, Reduction>(
            this IEnumerable<Input> input,
            Reduction reduction,
            IReducer<Reduction, Input> reducer)
        {
            foreach (var value in input)
            {
                var terminator = reducer.Invoke(reduction, value);

                if(terminator.Terminated)
                {
                    return terminator;
                }
            }

            return reducer.Complete(reduction);
        }

        public static IEnumerable<Result> Transduce<Input, Result>(
            this IEnumerable<Input> input,
            ITransducer<Input, Result> transuducer)
        {
            var reducer = transuducer.Apply(Collection.ListReducer<Result>());
            var list = new List<Result>();

            foreach (var value in input)
            {
                var reduction = reducer.Invoke(list, value);

                foreach (var result in reduction.Value)
                {
                    yield return result;
                }

                if (reduction.Terminated)
                {
                    yield break;
                }

                list.Clear();
            }

            var completionReduction = reducer.Complete(new List<Result>());
            foreach (var result in completionReduction.Value)
            {
                yield return result;
            }
        }
        
        public static IList<Result> Collect<Input, Result>(
            this IEnumerable<Input> input,
            ITransducer<Input, Result> transducer) =>
            input.Reduce(new List<Result>(), transducer.Apply(Collection.ListReducer<Result>())).Value;
    }
}
