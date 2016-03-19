namespace TD
{
    public interface ICompletion<Reduction>
    {
        Terminator<Reduction> Complete(Reduction reduction);
    }

    public interface IReducer<Reduction, Input> : ICompletion<Reduction>
    {
        Terminator<Reduction> Invoke(Reduction reduction, Input value);
    }

    public abstract class DefaultCompletionReducer<Reduction, Input> : IReducer<Reduction, Input>
    {
        protected ICompletion<Reduction> Completion { get; private set; }

        public DefaultCompletionReducer(ICompletion<Reduction> completion)
        {
            Completion = completion;
        }

        public Terminator<Reduction> Complete(Reduction reduction) => Completion.Complete(reduction);

        public abstract Terminator<Reduction> Invoke(Reduction reduction, Input value);
    }
}
