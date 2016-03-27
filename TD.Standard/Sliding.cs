using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TD.Terminator;

namespace TD
{
    internal class Sliding<T> : ITransducer<T, IList<T>>
    {
        class Reducer<TReduction> : DefaultCompletionReducer<TReduction, T, IList<T>>
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
                if (WindowFill < Window.Length)
                {
                    Window[WindowFill++] = value;
                }
                else
                {
                    for (var i = 0; i < Window.Length - 1; i++)
                    {
                        Window[i] = Window[i + 1];
                    }
                    Window[Window.Length - 1] = value;
                }

                if (WindowFill < Window.Length)
                {
                    return Reduction(reduction);
                }

                return Next.Invoke(reduction, Window);
            }
        }

        public int Window { get; private set; }

        public Sliding(int window)
        {
            Window = window;
        }

        public IReducer<TReduction, T> Apply<TReduction>(IReducer<TReduction, IList<T>> next) =>
            new Reducer<TReduction>(Window, next);
    }
}
