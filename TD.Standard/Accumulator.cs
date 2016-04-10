using System;
using System.Threading.Tasks;
using static TD.Terminator;

namespace TD
{
    internal abstract class BaseAccumulationTransducer<T> : ITransducer<T, T>
    {
        private class Reducer<TReduction> : BaseReducer<TReduction, T, T>
        {
            private T Accumulation = default(T);
            private readonly BaseAccumulationTransducer<T> Transducer;

            public Reducer(
                BaseAccumulationTransducer<T> transducer,
                IReducer<TReduction, T> next) : base(next)
            {
                Transducer = transducer;
            }

            public override Terminator<TReduction> Complete(TReduction reduction)
            {
                var terminator = Next.Invoke(reduction, Accumulation);

                return terminator.IsTerminated
                    ? terminator
                    : Next.Complete(terminator.Value);
            }

            public override Terminator<TReduction> Invoke(TReduction reduction, T value)
            {
                Accumulation = Transducer.Add(Accumulation, value);
                return Reduction(reduction);
            }
        }

        private class AsyncReducer<TReduction> : BaseAsyncReducer<TReduction, T, T>
        {
            private T Accumulation;
            private readonly BaseAccumulationTransducer<T> Transducer;

            public AsyncReducer(
                BaseAccumulationTransducer<T> transducer,
                IAsyncReducer<TReduction, T> next) : base(next)
            {
                Transducer = transducer;
            }

            public override async Task<Terminator<TReduction>> CompleteAsync(TReduction reduction)
            {
                var terminator = await Next.InvokeAsync(reduction, Accumulation).ConfigureAwait(false);
                
                return terminator.IsTerminated 
                    ? terminator 
                    : await Next.CompleteAsync(terminator.Value).ConfigureAwait(false);
            }

            public override Task<Terminator<TReduction>> InvokeAsync(TReduction reduction, T value)
            {
                Accumulation = Transducer.Add(Accumulation, value);
                return Task.FromResult(Reduction(reduction));
            }
        }
        
        public abstract T Add(T reduction, T value);
        
        public ITransducer<U, U> As<U>() => this as ITransducer<U, U>;

        public IAsyncReducer<TReduction, T> Apply<TReduction>(IAsyncReducer<TReduction, T> next) =>
            new AsyncReducer<TReduction>(this, next);

        public IReducer<TReduction, T> Apply<TReduction>(IReducer<TReduction, T> next) =>
            new Reducer<TReduction>(this, next);
    }

    internal class UncheckedSByteAccumulator : BaseAccumulationTransducer<sbyte>
    {
        public override sbyte Add(sbyte reduction, sbyte value) => (sbyte)(reduction + value);
    }

    internal class UncheckedByteAccumulator : BaseAccumulationTransducer<byte>
    {
        public override byte Add(byte reduction, byte value) => (byte)(reduction + value);
    }

    internal class UncheckedInt16Accumulator : BaseAccumulationTransducer<short>
    {
        public override short Add(short reduction, short value) => (short)(reduction + value);
    }

    internal class UncheckedUint16Accumulator : BaseAccumulationTransducer<ushort>
    {
        public override ushort Add(ushort reduction, ushort value) => (ushort)(reduction + value);
    }

    internal class UncheckedInt32Accumulator : BaseAccumulationTransducer<int>
    {
        public override int Add(int reduction, int value) => reduction + value;
    }

    internal class UncheckedUint32Accumulator : BaseAccumulationTransducer<uint>
    {
        public override uint Add(uint reduction, uint value) => reduction + value;
    }

    internal class UncheckedInt64Accumulator : BaseAccumulationTransducer<long>
    {
        public override long Add(long reduction, long value) => reduction + value;
    }

    internal class UncheckedUint64Accumulator : BaseAccumulationTransducer<ulong>
    {
        public override ulong Add(ulong reduction, ulong value) => reduction + value;
    }

    internal class UncheckedFloatAccumulator : BaseAccumulationTransducer<float>
    {
        public override float Add(float reduction, float value) => reduction + value;
    }

    internal class UncheckedDoubleAccumulator : BaseAccumulationTransducer<double>
    {
        public override double Add(double reduction, double value) => reduction + value;
    }

    internal class UncheckedDecimalAccumulator : BaseAccumulationTransducer<decimal>
    {
        public override decimal Add(decimal reduction, decimal value) => reduction + value;
    }

    internal class CheckedSByteAccumulator : BaseAccumulationTransducer<sbyte>
    {
        public override sbyte Add(sbyte reduction, sbyte value)
        {
            checked
            {
                return (sbyte)(reduction + value);
            }
        }
    }

    internal class CheckedByteAccumulator : BaseAccumulationTransducer<byte>
    {
        public override byte Add(byte reduction, byte value)
        {
            checked
            {
                return (byte)(reduction + value);
            }
        }
    }

    internal class CheckedInt16Accumulator : BaseAccumulationTransducer<short>
    {
        public override short Add(short reduction, short value)
        {
            checked
            {
                return (short)(reduction + value);
            }
        }
    }

    internal class CheckedUint16Accumulator : BaseAccumulationTransducer<ushort>
    {
        public override ushort Add(ushort reduction, ushort value)
        {
            checked
            {
                return (ushort)(reduction + value);
            }
        }
    }

    internal class CheckedInt32Accumulator : BaseAccumulationTransducer<int>
    {
        public override int Add(int reduction, int value) => checked(reduction + value);
    }

    internal class CheckedUint32Accumulator : BaseAccumulationTransducer<uint>
    {
        public override uint Add(uint reduction, uint value) => checked(reduction + value);
    }

    internal class CheckedInt64Accumulator : BaseAccumulationTransducer<long>
    {
        public override long Add(long reduction, long value) => checked(reduction + value);
    }

    internal class CheckedUint64Accumulator : BaseAccumulationTransducer<ulong>
    {
        public override ulong Add(ulong reduction, ulong value) => checked(reduction + value);
    }

    internal class CheckedFloatAccumulator : BaseAccumulationTransducer<float>
    {
        public override float Add(float reduction, float value) => checked(reduction + value);
    }

    internal class CheckedDoubleAccumulator : BaseAccumulationTransducer<double>
    {
        public override double Add(double reduction, double value) => checked(reduction + value);
    }

    internal class CheckedDecimalAccumulator : BaseAccumulationTransducer<decimal>
    {
        public override decimal Add(decimal reduction, decimal value) => checked(reduction + value);
    }

    public static class Accumulating
    {
        public static ITransducer<T, T> Unchecked<T>() where T : struct
        {
            if (typeof(T) == typeof(sbyte)) return new UncheckedSByteAccumulator().As<T>();
            if (typeof(T) == typeof(byte)) return new UncheckedByteAccumulator().As<T>();
            if (typeof(T) == typeof(short)) return new UncheckedInt16Accumulator().As<T>();
            if (typeof(T) == typeof(ushort)) return new UncheckedUint16Accumulator().As<T>();
            if (typeof(T) == typeof(int)) return new UncheckedInt32Accumulator().As<T>();
            if (typeof(T) == typeof(uint)) return new UncheckedUint32Accumulator().As<T>();
            if (typeof(T) == typeof(long)) return new UncheckedInt64Accumulator().As<T>();
            if (typeof(T) == typeof(ulong)) return new UncheckedUint64Accumulator().As<T>();
            if (typeof(T) == typeof(float)) return new UncheckedFloatAccumulator().As<T>();
            if (typeof(T) == typeof(double)) return new UncheckedDoubleAccumulator().As<T>();
            if (typeof(T) == typeof(decimal)) return new UncheckedDecimalAccumulator().As<T>();

            throw new NotImplementedException($"{nameof(Accumulator.Unchecked)} not implemented for type {typeof(T)}.");
        }

        public static ITransducer<T, T> Checked<T>() where T : struct
        {
            if (typeof(T) == typeof(sbyte)) return new CheckedSByteAccumulator().As<T>();
            if (typeof(T) == typeof(byte)) return new CheckedByteAccumulator().As<T>();
            if (typeof(T) == typeof(short)) return new CheckedInt16Accumulator().As<T>();
            if (typeof(T) == typeof(ushort)) return new CheckedUint16Accumulator().As<T>();
            if (typeof(T) == typeof(int)) return new CheckedInt32Accumulator().As<T>();
            if (typeof(T) == typeof(uint)) return new CheckedUint32Accumulator().As<T>();
            if (typeof(T) == typeof(long)) return new CheckedInt64Accumulator().As<T>();
            if (typeof(T) == typeof(ulong)) return new CheckedUint64Accumulator().As<T>();
            if (typeof(T) == typeof(float)) return new CheckedFloatAccumulator().As<T>();
            if (typeof(T) == typeof(double)) return new CheckedDoubleAccumulator().As<T>();
            if (typeof(T) == typeof(decimal)) return new CheckedDecimalAccumulator().As<T>();

            throw new NotImplementedException($"{nameof(Accumulator.Checked)} not implemented for type {typeof(T)}.");
        }
    }

    public static class Accumulator
    {
        public static IReducer<T, T> Unchecked<T>() where T : struct =>
            Accumulating.Unchecked<T>().Apply(Reducer.Make<T, T>((acc, val) => val));

        public static IReducer<T, T> Checked<T>() where T : struct =>
            Accumulating.Checked<T>().Apply(Reducer.Make<T, T>((acc, val) => val));
    }
}
