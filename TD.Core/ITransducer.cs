namespace TD
{
    public interface ITransducer<From, To>
    {
        IReducer<Reduction, From> Apply<Reduction>(IReducer<Reduction, To> reducer);
    }
}
