using System;
using static System.Net.IPAddress;

namespace TD
{
    internal abstract class NetworkOrderReducer<TReduction, T> : DefaultCompletionReducer<TReduction, T, T> where T : struct
    {
        protected NetworkOrderReducer(IReducer<TReduction, T> next) : base(next) { }

        public override Terminator<TReduction> Invoke(TReduction reduction, T value) => Next.Invoke(reduction, value);

        protected abstract T NetworkOrder(T hostOrder);

        public IReducer<TReduction, U> As<U>() => this as IReducer<TReduction, U>;
    }

    #region Signed
    internal class Int16NetworkOrder<TReduction> : NetworkOrderReducer<TReduction, short>
    {
        public Int16NetworkOrder(IReducer<TReduction, short> next) : base(next) { }

        protected override short NetworkOrder(short hostOrder) => HostToNetworkOrder(hostOrder);
    }

    internal class Int32NetworkOrder<TReduction> : NetworkOrderReducer<TReduction, int>
    {
        public Int32NetworkOrder(IReducer<TReduction, int> next) : base(next) { }

        protected override int NetworkOrder(int hostOrder) => HostToNetworkOrder(hostOrder);
    }

    internal class Int64NetworkOrder<TReduction> : NetworkOrderReducer<TReduction, long>
    {
        public Int64NetworkOrder(IReducer<TReduction, long> next) : base(next) { }

        protected override long NetworkOrder(long hostOrder) => HostToNetworkOrder(hostOrder);
    }
    #endregion

    #region Unsigned
    internal class UInt16NetworkOrder<TReduction> : NetworkOrderReducer<TReduction, ushort>
    {
        public UInt16NetworkOrder(IReducer<TReduction, ushort> next) : base(next) { }

        protected override ushort NetworkOrder(ushort hostOrder) =>
            unchecked((ushort)HostToNetworkOrder(unchecked((short)hostOrder)));
    }

    internal class UInt32NetworkOrder<TReduction> : NetworkOrderReducer<TReduction, uint>
    {
        public UInt32NetworkOrder(IReducer<TReduction, uint> next) : base(next) { }

        protected override uint NetworkOrder(uint hostOrder) =>
            unchecked((uint)HostToNetworkOrder(unchecked((int)hostOrder)));
    }

    internal class UInt64NetworkOrder<TReduction> : NetworkOrderReducer<TReduction, ulong>
    {
        public UInt64NetworkOrder(IReducer<TReduction, ulong> next) : base(next) { }

        protected override ulong NetworkOrder(ulong hostOrder) => 
            unchecked((ulong)HostToNetworkOrder(unchecked((long)hostOrder)));
    }
    #endregion

    internal class NetworkOrder<T> : ITransducer<T, T> where T : struct
    {
        public IReducer<TReduction, T> Apply<TReduction>(IReducer<TReduction, T> next)
        {
            #region Unsigned
            if (typeof(T) == typeof(byte)) return next;
            if (typeof(T) == typeof(ushort))
                return new UInt16NetworkOrder<TReduction>((IReducer<TReduction, ushort>)next).As<T>();
            if (typeof(T) == typeof(uint))
                return new UInt32NetworkOrder<TReduction>((IReducer<TReduction, uint>)next).As<T>();
            if (typeof(T) == typeof(uint))
                return new UInt64NetworkOrder<TReduction>((IReducer<TReduction, ulong>)next).As<T>();
            #endregion

            #region Signed
            if (typeof(T) == typeof(sbyte)) return next;
            if (typeof(T) == typeof(short))
                return new Int16NetworkOrder<TReduction>((IReducer<TReduction, short>)next).As<T>();
            if (typeof(T) == typeof(int))
                return new Int32NetworkOrder<TReduction>((IReducer<TReduction, int>)next).As<T>();
            if (typeof(T) == typeof(long))
                return new Int64NetworkOrder<TReduction>((IReducer<TReduction, long>)next).As<T>();
            #endregion

            throw new NotImplementedException($"Network Order is not implemented for {typeof(T)}.");
        }
    }
}
