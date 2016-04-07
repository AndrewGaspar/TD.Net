using System;
using static System.Net.IPAddress;

namespace TD
{
    internal abstract class HostOrderReducer<TReduction, T> : DefaultCompletionReducer<TReduction, T, T> where T : struct
    {
        protected HostOrderReducer(IReducer<TReduction, T> next) : base(next) { }

        public override Terminator<TReduction> Invoke(TReduction reduction, T value) => Next.Invoke(reduction, value);

        protected abstract T HostOrder(T hostOrder);

        public IReducer<TReduction, U> As<U>() => this as IReducer<TReduction, U>;
    }

    #region Signed
    internal class Int16HostOrder<TReduction> : HostOrderReducer<TReduction, short>
    {
        public Int16HostOrder(IReducer<TReduction, short> next) : base(next) { }

        protected override short HostOrder(short hostOrder) => NetworkToHostOrder(hostOrder);
    }

    internal class Int32HostOrder<TReduction> : HostOrderReducer<TReduction, int>
    {
        public Int32HostOrder(IReducer<TReduction, int> next) : base(next) { }

        protected override int HostOrder(int hostOrder) => NetworkToHostOrder(hostOrder);
    }

    internal class Int64HostOrder<TReduction> : HostOrderReducer<TReduction, long>
    {
        public Int64HostOrder(IReducer<TReduction, long> next) : base(next) { }

        protected override long HostOrder(long hostOrder) => NetworkToHostOrder(hostOrder);
    }
    #endregion

    #region Unsigned
    internal class UInt16HostOrder<TReduction> : HostOrderReducer<TReduction, ushort>
    {
        public UInt16HostOrder(IReducer<TReduction, ushort> next) : base(next) { }

        protected override ushort HostOrder(ushort hostOrder) =>
            unchecked((ushort)NetworkToHostOrder(unchecked((short)hostOrder)));
    }

    internal class UInt32HostOrder<TReduction> : HostOrderReducer<TReduction, uint>
    {
        public UInt32HostOrder(IReducer<TReduction, uint> next) : base(next) { }

        protected override uint HostOrder(uint hostOrder) =>
            unchecked((uint)NetworkToHostOrder(unchecked((int)hostOrder)));
    }

    internal class UInt64HostOrder<TReduction> : HostOrderReducer<TReduction, ulong>
    {
        public UInt64HostOrder(IReducer<TReduction, ulong> next) : base(next) { }

        protected override ulong HostOrder(ulong hostOrder) =>
            unchecked((ulong)NetworkToHostOrder(unchecked((long)hostOrder)));
    }
    #endregion

    internal class HostOrder<T> : ITransducer<T, T> where T : struct
    {
        public IReducer<TReduction, T> Apply<TReduction>(IReducer<TReduction, T> next)
        {
            #region Unsigned
            if (typeof(T) == typeof(byte)) return next;
            if (typeof(T) == typeof(ushort))
                return new UInt16HostOrder<TReduction>((IReducer<TReduction, ushort>)next).As<T>();
            if (typeof(T) == typeof(uint))
                return new UInt32HostOrder<TReduction>((IReducer<TReduction, uint>)next).As<T>();
            if (typeof(T) == typeof(uint))
                return new UInt64HostOrder<TReduction>((IReducer<TReduction, ulong>)next).As<T>();
            #endregion

            #region Signed
            if (typeof(T) == typeof(sbyte)) return next;
            if (typeof(T) == typeof(short))
                return new Int16HostOrder<TReduction>((IReducer<TReduction, short>)next).As<T>();
            if (typeof(T) == typeof(int))
                return new Int32HostOrder<TReduction>((IReducer<TReduction, int>)next).As<T>();
            if (typeof(T) == typeof(long))
                return new Int64HostOrder<TReduction>((IReducer<TReduction, long>)next).As<T>();
            #endregion

            throw new NotImplementedException($"Network Order is not implemented for {typeof(T)}.");
        }
    }
}
