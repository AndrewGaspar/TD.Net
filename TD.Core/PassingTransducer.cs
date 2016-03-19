namespace TD
{
    class PassingTransducer<T> : ITransducer<T, T>
    {
        public IReducer<Reduction, T> Apply<Reduction>(IReducer<Reduction, T> next) => next;
    }
}
