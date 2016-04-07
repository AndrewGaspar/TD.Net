using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TD.Core;

namespace TD
{
    internal abstract class NumericToBytesReducer<TReduction, T> : DefaultCompletionReducer<TReduction, T, byte[]>
        where T : struct
    {
        protected NumericToBytesReducer(IReducer<TReduction, byte[]> next) : base(next) { }

        public override Terminator<TReduction> Invoke(TReduction reduction, T value) =>
            Next.Invoke(reduction, Convert(value));

        protected abstract byte[] Convert(T value);

        public IReducer<TReduction, U> As<U>() => this as IReducer<TReduction, U>;
    }

    #region Signed
    internal class Int16ToBytesReducer<TReduction> : NumericToBytesReducer<TReduction, short>
    {
        public Int16ToBytesReducer(IReducer<TReduction, byte[]> next) : base(next) { }

        protected override byte[] Convert(short value) => BitConverter.GetBytes(value);
    }

    internal class Int32ToBytesReducer<TReduction> : NumericToBytesReducer<TReduction, int>
    {
        public Int32ToBytesReducer(IReducer<TReduction, byte[]> next) : base(next) { }

        protected override byte[] Convert(int value) => BitConverter.GetBytes(value);
    }

    internal class Int64ToBytesReducer<TReduction> : NumericToBytesReducer<TReduction, long>
    {
        public Int64ToBytesReducer(IReducer<TReduction, byte[]> next) : base(next) { }

        protected override byte[] Convert(long value) => BitConverter.GetBytes(value);
    }
    #endregion

    #region Unsigned
    internal class UInt16ToBytesReducer<TReduction> : NumericToBytesReducer<TReduction, ushort>
    {
        public UInt16ToBytesReducer(IReducer<TReduction, byte[]> next) : base(next) { }

        protected override byte[] Convert(ushort value) => BitConverter.GetBytes(value);
    }

    internal class UInt32ToBytesReducer<TReduction> : NumericToBytesReducer<TReduction, uint>
    {
        public UInt32ToBytesReducer(IReducer<TReduction, byte[]> next) : base(next) { }

        protected override byte[] Convert(uint value) => BitConverter.GetBytes(value);
    }

    internal class UInt64ToBytesReducer<TReduction> : NumericToBytesReducer<TReduction, ulong>
    {
        public UInt64ToBytesReducer(IReducer<TReduction, byte[]> next) : base(next) { }

        protected override byte[] Convert(ulong value) => BitConverter.GetBytes(value);
    }
    #endregion
    
    #region Floating
    internal class SingleToBytesReducer<TReduction> : NumericToBytesReducer<TReduction, float>
    {
        public SingleToBytesReducer(IReducer<TReduction, byte[]> next) : base(next) { }

        protected override byte[] Convert(float value) => BitConverter.GetBytes(value);
    }

    internal class DoubleToBytesReducer<TReduction> : NumericToBytesReducer<TReduction, double>
    {
        public DoubleToBytesReducer(IReducer<TReduction, byte[]> next) : base(next) { }

        protected override byte[] Convert(double value) => BitConverter.GetBytes(value);
    }
    #endregion

    internal class CharToBytesReducer<TReduction> : NumericToBytesReducer<TReduction, char>
    {
        public CharToBytesReducer(IReducer<TReduction, byte[]> next) : base(next) { }

        protected override byte[] Convert(char value) => BitConverter.GetBytes(value);
    }

    internal class BooleanToBytesReducer<TReduction> : NumericToBytesReducer<TReduction, bool>
    {
        public BooleanToBytesReducer(IReducer<TReduction, byte[]> next) : base(next) { }

        protected override byte[] Convert(bool value) => BitConverter.GetBytes(value);
    }

    internal class ConvertToBytes<T> : ITransducer<T, byte[]> where T : struct
    {
        public IReducer<TReduction, T> Apply<TReduction>(IReducer<TReduction, byte[]> next)
        {
            #region Unsigned
            if (typeof(T) == typeof(byte))
                return Mapping<byte, byte[]>(b => new[] { b }).Apply(next) as IReducer<TReduction, T>;
            if (typeof(T) == typeof(ushort)) return new UInt16ToBytesReducer<TReduction>(next).As<T>();
            if (typeof(T) == typeof(uint)) return new UInt32ToBytesReducer<TReduction>(next).As<T>();
            if (typeof(T) == typeof(ulong)) return new UInt64ToBytesReducer<TReduction>(next).As<T>();
            #endregion

            #region Signed
            if (typeof(T) == typeof(sbyte))
                return Mapping<sbyte, byte[]>(b => new[] { unchecked((byte)b) }).Apply(next) as IReducer<TReduction, T>;
            if (typeof(T) == typeof(short)) return new Int16ToBytesReducer<TReduction>(next).As<T>();
            if (typeof(T) == typeof(int)) return new Int32ToBytesReducer<TReduction>(next).As<T>();
            if (typeof(T) == typeof(long)) return new Int64ToBytesReducer<TReduction>(next).As<T>();
            #endregion

            #region Floating
            if (typeof(T) == typeof(float)) return new SingleToBytesReducer<TReduction>(next).As<T>();
            if (typeof(T) == typeof(double)) return new DoubleToBytesReducer<TReduction>(next).As<T>();
            #endregion

            if (typeof(T) == typeof(char)) return new CharToBytesReducer<TReduction>(next).As<T>();
            if (typeof(T) == typeof(bool)) return new BooleanToBytesReducer<TReduction>(next).As<T>();

            throw new NotImplementedException($"BitConvert not implemented for type {typeof(T)}");
        }
    }

    internal abstract class BytesToNumericReducer<TReduction, T> : DefaultCompletionReducer<TReduction, IEnumerable<byte>, T>
        where T : struct
    {
        protected BytesToNumericReducer(IReducer<TReduction, T> next) : base(next) { }

        public override Terminator<TReduction> Invoke(TReduction reduction, IEnumerable<byte> value) =>
            Next.Invoke(reduction, Convert(value));

        protected abstract T Convert(IEnumerable<byte> bytes);
    }

    #region Signed
    internal class BytesToInt16Reducer<TReduction> : BytesToNumericReducer<TReduction, short>
    {
        public BytesToInt16Reducer(IReducer<TReduction, short> next) : base(next) { }

        protected override short Convert(IEnumerable<byte> bytes) =>
            BitConverter.ToInt16(bytes.Take(sizeof(short)).ToArray(), startIndex: 0);
    }

    internal class BytesToInt32Reducer<TReduction> : BytesToNumericReducer<TReduction, int>
    {
        public BytesToInt32Reducer(IReducer<TReduction, int> next) : base(next) { }

        protected override int Convert(IEnumerable<byte> bytes) =>
            BitConverter.ToInt32(bytes.Take(sizeof(int)).ToArray(), startIndex: 0);
    }

    internal class BytesToInt64Reducer<TReduction> : BytesToNumericReducer<TReduction, long>
    {
        public BytesToInt64Reducer(IReducer<TReduction, long> next) : base(next) { }

        protected override long Convert(IEnumerable<byte> bytes) =>
            BitConverter.ToInt64(bytes.Take(sizeof(long)).ToArray(), startIndex: 0);
    }
    #endregion

    #region Unsigned
    internal class BytesToUInt16Reducer<TReduction> : BytesToNumericReducer<TReduction, ushort>
    {
        public BytesToUInt16Reducer(IReducer<TReduction, ushort> next) : base(next) { }

        protected override ushort Convert(IEnumerable<byte> bytes) =>
            BitConverter.ToUInt16(bytes.Take(sizeof(ushort)).ToArray(), startIndex: 0);
    }

    internal class BytesToUInt32Reducer<TReduction> : BytesToNumericReducer<TReduction, uint>
    {
        public BytesToUInt32Reducer(IReducer<TReduction, uint> next) : base(next) { }

        protected override uint Convert(IEnumerable<byte> bytes) =>
            BitConverter.ToUInt32(bytes.Take(sizeof(uint)).ToArray(), startIndex: 0);
    }

    internal class BytesToUInt64Reducer<TReduction> : BytesToNumericReducer<TReduction, ulong>
    {
        public BytesToUInt64Reducer(IReducer<TReduction, ulong> next) : base(next) { }

        protected override ulong Convert(IEnumerable<byte> bytes) =>
            BitConverter.ToUInt64(bytes.Take(sizeof(ulong)).ToArray(), startIndex: 0);
    }
    #endregion

    #region Floating
    internal class BytesToSingleReducer<TReduction> : BytesToNumericReducer<TReduction, float>
    {
        public BytesToSingleReducer(IReducer<TReduction, float> next) : base(next) { }

        protected override float Convert(IEnumerable<byte> bytes) =>
            BitConverter.ToSingle(bytes.Take(sizeof(float)).ToArray(), startIndex: 0);
    }

    internal class BytesToDoubleReducer<TReduction> : BytesToNumericReducer<TReduction, double>
    {
        public BytesToDoubleReducer(IReducer<TReduction, double> next) : base(next) { }

        protected override double Convert(IEnumerable<byte> bytes) =>
            BitConverter.ToDouble(bytes.Take(sizeof(double)).ToArray(), startIndex: 0);
    }
    #endregion
    internal class BytesToCharReducer<TReduction> : BytesToNumericReducer<TReduction, char>
    {
        public BytesToCharReducer(IReducer<TReduction, char> next) : base(next) { }

        protected override char Convert(IEnumerable<byte> bytes) =>
            BitConverter.ToChar(bytes.Take(sizeof(char)).ToArray(), startIndex: 0);
    }

    internal class BytesToBooleanReducer<TReduction> : BytesToNumericReducer<TReduction, bool>
    {
        public BytesToBooleanReducer(IReducer<TReduction, bool> next) : base(next) { }

        protected override bool Convert(IEnumerable<byte> bytes) =>
            BitConverter.ToBoolean(bytes.Take(sizeof(bool)).ToArray(), startIndex: 0);
    }

    internal class ConvertToNumeric<T> : ITransducer<IEnumerable<byte>, T> where T : struct
    {
        public IReducer<TReduction, IEnumerable<byte>> Apply<TReduction>(IReducer<TReduction, T> next)
        {
            #region Unsigned
            if (typeof(T) == typeof(byte))
                return Mapping<IEnumerable<byte>, byte>(b => b.First()).Apply(next as IReducer<TReduction, byte>);
            if (typeof(T) == typeof(ushort))
                return new BytesToUInt16Reducer<TReduction>(next as IReducer<TReduction, ushort>);
            if (typeof(T) == typeof(uint))
                return new BytesToUInt32Reducer<TReduction>(next as IReducer<TReduction, uint>);
            if (typeof(T) == typeof(ulong))
                return new BytesToUInt64Reducer<TReduction>(next as IReducer<TReduction, ulong>);
            #endregion

            #region Signed
            if (typeof(T) == typeof(sbyte))
                return Mapping<IEnumerable<byte>, sbyte>(b => unchecked((sbyte)b.First())).Apply(next as IReducer<TReduction, sbyte>);
            if (typeof(T) == typeof(short))
                return new BytesToInt16Reducer<TReduction>(next as IReducer<TReduction, short>);
            if (typeof(T) == typeof(int))
                return new BytesToInt32Reducer<TReduction>(next as IReducer<TReduction, int>);
            if (typeof(T) == typeof(long))
                return new BytesToInt64Reducer<TReduction>(next as IReducer<TReduction, long>);
            #endregion

            #region Floating
            if (typeof(T) == typeof(float))
                return new BytesToSingleReducer<TReduction>(next as IReducer<TReduction, float>);
            if (typeof(T) == typeof(double))
                return new BytesToDoubleReducer<TReduction>(next as IReducer<TReduction, double>);
            #endregion

            if (typeof(T) == typeof(char))
                return new BytesToCharReducer<TReduction>(next as IReducer<TReduction, char>);
            if (typeof(T) == typeof(bool))
                return new BytesToBooleanReducer<TReduction>(next as IReducer<TReduction, bool>);

            throw new NotImplementedException($"BitConvert not implemented for type {typeof(T)}");
        }
    }
}
