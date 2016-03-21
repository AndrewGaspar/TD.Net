using System;

namespace TD
{
    internal class FilteringTransducer<Input> : ITransducer<Input, Input>
    {
        private class FilteringReducer<Reduction> : DefaultCompletionReducer<Reduction, Input, Input>
        {
            public FilteringTransducer<Input> Transducer { get; private set; }

            public FilteringReducer(
                FilteringTransducer<Input> transducer,
                IReducer<Reduction, Input> reducer) : base(reducer)
            {
                Transducer = transducer;
            }

            public override Terminator<Reduction> Invoke(Reduction reduction, Input value) =>
                Transducer.Test(value) ? Next.Invoke(reduction, value) : Terminator.Reduction(reduction);
        }

        public Predicate<Input> Test { get; private set; }

        public FilteringTransducer(Predicate<Input> test)
        {
            Test = test;
        }

        public IReducer<Reduction, Input> Apply<Reduction>(IReducer<Reduction, Input> reducer)
        {
            return new FilteringReducer<Reduction>(this, reducer);
        }
    }
}
