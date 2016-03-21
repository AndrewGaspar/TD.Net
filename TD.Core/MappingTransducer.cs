using System;

namespace TD
{
    internal class MappingTransducer<From, To> : ITransducer<From, To>
    {
        private class MappingReducer<Reduction> : DefaultCompletionReducer<Reduction, From, To>
        {
            public MappingTransducer<From, To> Transducer { get; private set; }

            public MappingReducer(
                MappingTransducer<From, To> transducer,
                IReducer<Reduction, To> reducer) : base(reducer)
            {
                Transducer = transducer;
            }

            public override Terminator<Reduction> Invoke(Reduction reduction, From value) =>
                Next.Invoke(reduction, Transducer.MappingFunction(value));
        }

        public Func<From, To> MappingFunction { get; private set; }

        public MappingTransducer(Func<From, To> func)
        {
            MappingFunction = func;
        }

        public IReducer<Reduction, From> Apply<Reduction>(IReducer<Reduction, To> reducer) =>
            new MappingReducer<Reduction>(this, reducer);
    }
}
