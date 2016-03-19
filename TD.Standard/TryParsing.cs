using System;

namespace TD
{
    public abstract class BaseParser<Reduction, Output> : IReducer<Reduction, string>
        where Output : struct
    {
        private IReducer<Reduction, Output?> next;

        public BaseParser(IReducer<Reduction, Output?> next)
        {
            this.next = next;
        }

        public Terminator<Reduction> Complete(Reduction reduction) => next.Complete(reduction);

        public Terminator<Reduction> Invoke(Reduction reduction, string value)
        {
            Output x;
            if (TryParse(value, out x))
            {
                return next.Invoke(reduction, x);
            }
            else
            {
                return next.Invoke(reduction, null);
            }
        }

        protected abstract bool TryParse(string value, out Output result);
    }

    public class Int32Parser<Reduction> : BaseParser<Reduction, Int32>
    {
        public Int32Parser(IReducer<Reduction, Int32?> next) : base(next) { }

        protected override bool TryParse(string value, out int result) => Int32.TryParse(value, out result);
    }

    public class TryParsing<T> : ITransducer<string, T?> where T : struct
    {
        public IReducer<Reduction, string> Apply<Reduction>(IReducer<Reduction, T?> reducer)
        {
            if(typeof(T) == typeof(Int32))
            {
                return new Int32Parser<Reduction>((IReducer<Reduction, Int32?>)reducer);
            }

            throw new NotImplementedException($"TryParsing is not implemented for type {typeof(T)}");
        }
    }
}
