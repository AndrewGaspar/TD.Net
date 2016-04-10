using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static TD.Terminator;

namespace TD
{
    internal class Multiplexing<TInput, TResult> : ITransducer<TInput, TResult>
    {
        private class Demux<TReduction> : IReducer<TReduction, TInput>
        {
            private readonly IList<IReducer<TReduction, TInput>> Reducers;

            public Demux(IList<IReducer<TReduction, TInput>> reducers)
            {
                Reducers = reducers;
            }

            private Terminator<TReduction> EachReducer(
                TReduction reduction,
                Func<TReduction, IReducer<TReduction, TInput>, Terminator<TReduction>> func)
            {
                return Reducers.Reduce(Reduction(reduction), 
                    Reducer.Make<Terminator<TReduction>, IReducer<TReduction, TInput>>((terminator, reducer) =>
                    {
                        terminator = func(terminator.Value, reducer);
                        return Reduction(terminator, terminated: terminator.IsTerminated);
                    })).Value;
            }

            public Terminator<TReduction> Complete(TReduction start) =>
                EachReducer(start, (reduction, reducer) => reducer.Complete(reduction));

            public Terminator<TReduction> Invoke(TReduction start, TInput value) =>
                EachReducer(start, (reduction, reducer) => reducer.Invoke(reduction, value));
        }

        private class AsyncDemux<TReduction> : IAsyncReducer<TReduction, TInput>
        {
            private readonly IList<IAsyncReducer<TReduction, TInput>> Reducers;

            public AsyncDemux(IList<IAsyncReducer<TReduction, TInput>> reducers)
            {
                Reducers = reducers;
            }

            private async Task<Terminator<TReduction>> EachReducer(
                TReduction reduction,
                Func<TReduction, IAsyncReducer<TReduction, TInput>, Task<Terminator<TReduction>>> func)
            {
                return (await Reducers.ReduceAsync(Reduction(reduction),
                    Reducer.AsyncMake<Terminator<TReduction>, IAsyncReducer<TReduction, TInput>>(
                        async (terminator, reducer) =>
                        {
                            terminator = await func(terminator.Value, reducer);
                            return Reduction(terminator, terminated: terminator.IsTerminated);
                        }))).Value;
            }

            public Task<Terminator<TReduction>> CompleteAsync(TReduction start) =>
                EachReducer(start, (reduction, reducer) => reducer.CompleteAsync(reduction));

            public Task<Terminator<TReduction>> InvokeAsync(TReduction start, TInput value) =>
                EachReducer(start, (reduction, reducer) => reducer.InvokeAsync(reduction, value));
        }

        private readonly IList<ITransducer<TInput, TResult>> Transducers;

        public Multiplexing(IList<ITransducer<TInput, TResult>> transducers)
        {
            Transducers = transducers;
        }

        public IReducer<TReduction, TInput> Apply<TReduction>(IReducer<TReduction, TResult> next) =>
            new Demux<TReduction>(Transducers.Select(transducer => transducer.Apply(next)).ToList());

        public IAsyncReducer<TReduction, TInput> Apply<TReduction>(IAsyncReducer<TReduction, TResult> next)
        {
            throw new NotImplementedException();
        }
    }
}
