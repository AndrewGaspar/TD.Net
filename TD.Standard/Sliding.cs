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
        class WindowContainer
        {
            private int WindowFill = 0;
            public readonly T[] Window;

            public WindowContainer(int windowSize)
            {
                Window = new T[windowSize];
            }

            public bool Push(T value)
            {
                if(WindowFill < Window.Length)
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

                return WindowFill == Window.Length;
            }
        }

        class Reducer<TReduction> : DefaultCompletionReducer<TReduction, T, IList<T>>
        {
            private WindowContainer Container;

            public Reducer(
                int windowSize,
                IReducer<TReduction, IList<T>> next) : base(next)
            {
                Container = new WindowContainer(windowSize);
            }

            public override Terminator<TReduction> Invoke(TReduction reduction, T value) =>
                Container.Push(value) ? Next.Invoke(reduction, Container.Window) : Reduction(reduction);
        }

        class AsyncReducer<TReduction> : DefaultCompletionAsyncReducer<TReduction, T, IList<T>>
        {
            private WindowContainer Container;

            public AsyncReducer(
                int windowSize,
                IAsyncReducer<TReduction, IList<T>> next) : base(next)
            {
                Container = new WindowContainer(windowSize);
            }

            public override Task<Terminator<TReduction>> InvokeAsync(TReduction reduction, T value) =>
                Container.Push(value) ? Next.InvokeAsync(reduction, Container.Window) 
                                      : Task.FromResult(Reduction(reduction));
        }

        public int Window { get; private set; }

        public Sliding(int window)
        {
            Window = window;
        }

        public IReducer<TReduction, T> Apply<TReduction>(IReducer<TReduction, IList<T>> next) =>
            new Reducer<TReduction>(Window, next);

        public IAsyncReducer<TReduction, T> Apply<TReduction>(IAsyncReducer<TReduction, IList<T>> next) =>
            new AsyncReducer<TReduction>(Window, next);
    }
}
