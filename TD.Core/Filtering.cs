using System;

namespace TD
{
    internal class Filtering<Input> : ITransducer<Input, Input>
    {
        private class Reducer<Reduction> : DefaultCompletionReducer<Reduction, Input, Input>
        {
            private readonly Predicate<Input> Test;

            public Reducer(
                Predicate<Input> test,
                IReducer<Reduction, Input> reducer) : base(reducer)
            {
                Test = test;
            }

            public override Terminator<Reduction> Invoke(Reduction reduction, Input value) =>
                Test(value) ? Next.Invoke(reduction, value) : Terminator.Reduction(reduction);
        }

        public Predicate<Input> Test { get; private set; }

        public Filtering(Predicate<Input> test)
        {
            Test = test;
        }

        public IReducer<Reduction, Input> Apply<Reduction>(IReducer<Reduction, Input> reducer)
        {
            return new Reducer<Reduction>(Test, reducer);
        }
    }
}
