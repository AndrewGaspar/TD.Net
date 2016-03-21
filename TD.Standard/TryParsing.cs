using System;

namespace TD
{
    internal abstract class BaseParser<Reduction, Output> : IReducer<Reduction, string>
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

    internal class Int32Parser<Reduction> : BaseParser<Reduction, int>
    {
        public Int32Parser(IReducer<Reduction, int?> next) : base(next) { }

        protected override bool TryParse(string value, out int result) => int.TryParse(value, out result);
    }

    internal class TryParsing<T> : ITransducer<string, T?> where T : struct
    {
        public IReducer<Reduction, string> Apply<Reduction>(IReducer<Reduction, T?> reducer)
        {
            if(typeof(T) == typeof(int))
            {
                return new Int32Parser<Reduction>((IReducer<Reduction, int?>)reducer);
            }

            throw new NotImplementedException($"TryParsing is not implemented for type {typeof(T)}");
        }
    }
}
