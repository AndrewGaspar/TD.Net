using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TD.Terminator;

namespace TD
{
    internal abstract class BaseAccumulationReducer<T> : IReducer<T, T>
    {
        public Terminator<T> Complete(T reduction) => Reduction(reduction);
        public Terminator<T> Invoke(T reduction, T value) => Reduction(Add(reduction, value));
        public abstract T Add(T reduction, T value);

        public IReducer<U, U> As<U>() => this as IReducer<U, U>;
    }

    internal class UncheckedSByteAccumulator : BaseAccumulationReducer<sbyte>
    {
        public override sbyte Add(sbyte reduction, sbyte value) => (sbyte)(reduction + value);
    }

    internal class UncheckedByteAccumulator : BaseAccumulationReducer<byte>
    {
        public override byte Add(byte reduction, byte value) => (byte)(reduction + value);
    }

    internal class UncheckedInt16Accumulator : BaseAccumulationReducer<short>
    {
        public override short Add(short reduction, short value) => (short)(reduction + value);
    }

    internal class UncheckedUint16Accumulator : BaseAccumulationReducer<ushort>
    {
        public override ushort Add(ushort reduction, ushort value) => (ushort)(reduction + value);
    }

    internal class UncheckedInt32Accumulator : BaseAccumulationReducer<int>
    {
        public override int Add(int reduction, int value) => reduction + value;
    }

    internal class UncheckedUint32Accumulator : BaseAccumulationReducer<uint>
    {
        public override uint Add(uint reduction, uint value) => reduction + value;
    }

    internal class UncheckedInt64Accumulator : BaseAccumulationReducer<long>
    {
        public override long Add(long reduction, long value) => reduction + value;
    }

    internal class UncheckedUint64Accumulator : BaseAccumulationReducer<ulong>
    {
        public override ulong Add(ulong reduction, ulong value) => reduction + value;
    }

    internal class UncheckedFloatAccumulator : BaseAccumulationReducer<float>
    {
        public override float Add(float reduction, float value) => reduction + value;
    }

    internal class UncheckedDoubleAccumulator : BaseAccumulationReducer<double>
    {
        public override double Add(double reduction, double value) => reduction + value;
    }

    internal class UncheckedDecimalAccumulator : BaseAccumulationReducer<decimal>
    {
        public override decimal Add(decimal reduction, decimal value) => reduction + value;
    }

    internal class CheckedSByteAccumulator : BaseAccumulationReducer<sbyte>
    {
        public override sbyte Add(sbyte reduction, sbyte value)
        {
            checked
            {
                return (sbyte)(reduction + value);
            }
        }
    }

    internal class CheckedByteAccumulator : BaseAccumulationReducer<byte>
    {
        public override byte Add(byte reduction, byte value)
        {
            checked
            {
                return (byte)(reduction + value);
            }
        }
    }

    internal class CheckedInt16Accumulator : BaseAccumulationReducer<short>
    {
        public override short Add(short reduction, short value)
        {
            checked
            {
                return (short)(reduction + value);
            }
        }
    }

    internal class CheckedUint16Accumulator : BaseAccumulationReducer<ushort>
    {
        public override ushort Add(ushort reduction, ushort value)
        {
            checked
            {
                return (ushort)(reduction + value);
            }
        }
    }

    internal class CheckedInt32Accumulator : BaseAccumulationReducer<int>
    {
        public override int Add(int reduction, int value) => checked(reduction + value);
    }

    internal class CheckedUint32Accumulator : BaseAccumulationReducer<uint>
    {
        public override uint Add(uint reduction, uint value) => checked(reduction + value);
    }

    internal class CheckedInt64Accumulator : BaseAccumulationReducer<long>
    {
        public override long Add(long reduction, long value) => checked(reduction + value);
    }

    internal class CheckedUint64Accumulator : BaseAccumulationReducer<ulong>
    {
        public override ulong Add(ulong reduction, ulong value) => checked(reduction + value);
    }

    internal class CheckedFloatAccumulator : BaseAccumulationReducer<float>
    {
        public override float Add(float reduction, float value) => checked(reduction + value);
    }

    internal class CheckedDoubleAccumulator : BaseAccumulationReducer<double>
    {
        public override double Add(double reduction, double value) => checked(reduction + value);
    }

    internal class CheckedDecimalAccumulator : BaseAccumulationReducer<decimal>
    {
        public override decimal Add(decimal reduction, decimal value) => checked(reduction + value);
    }

    public static class Accumulator
    {
        public static IReducer<T, T> Unchecked<T>()
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

        public static IReducer<T, T> Checked<T>()
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
}
