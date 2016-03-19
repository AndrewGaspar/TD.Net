namespace TD
{
    internal class ComposingTransducer<From, Mid, To> : ITransducer<From, To>
    {
        public ITransducer<From, Mid> Left { get; private set; }
        public ITransducer<Mid, To> Right { get; private set; }

        public ComposingTransducer(
            ITransducer<From, Mid> left,
            ITransducer<Mid, To> right)
        {
            Left = left;
            Right = right;
        }

        public IReducer<Reduction, From> Apply<Reduction>(IReducer<Reduction, To> reducer) =>
            Left.Apply(Right.Apply(reducer));
    }
}
