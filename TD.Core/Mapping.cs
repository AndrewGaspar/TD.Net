using System;

namespace TD
{
    internal class Mapping<From, To> : ITransducer<From, To>
    {
        private class Reducer<Reduction> : DefaultCompletionReducer<Reduction, From, To>
        {
            private readonly Func<From, To> MappingFunction;

            public Reducer(
                Func<From, To> mappingFunction,
                IReducer<Reduction, To> reducer) : base(reducer)
            {
                MappingFunction = mappingFunction;
            }

            public override Terminator<Reduction> Invoke(Reduction reduction, From value) =>
                Next.Invoke(reduction, MappingFunction(value));
        }

        public Func<From, To> MappingFunction { get; private set; }

        public Mapping(Func<From, To> func)
        {
            MappingFunction = func;
        }

        public IReducer<Reduction, From> Apply<Reduction>(IReducer<Reduction, To> reducer) =>
            new Reducer<Reduction>(MappingFunction, reducer);
    }
}
