using System.Collections.Generic;
using static TD.Terminator;

namespace TD
{
    internal class Framing<T> : ITransducer<T, IList<T>>
    {
        private class Reducer<TReduction> : DefaultCompletionReducer<TReduction, T, IList<T>>
        {
            private int WindowFill = 0;
            private readonly T[] Window;

            public Reducer(
                int window,
                IReducer<TReduction, IList<T>> next) : base(next)
            {
                Window = new T[window];
            }

            public override Terminator<TReduction> Invoke(TReduction reduction, T value)
            {
                Window[WindowFill++] = value;

                if (WindowFill == Window.Length)
                {
                    WindowFill = 0;
                    return Next.Invoke(reduction, Window);
                }
                else
                {
                    return Reduction(reduction);
                }
            }
        }

        public int WindowSize { get; private set; }

        public Framing(int window)
        {
            WindowSize = window;
        }

        public IReducer<TReduction, T> Apply<TReduction>(IReducer<TReduction, IList<T>> next) =>
            new Reducer<TReduction>(WindowSize, next);
    }
}
