using System;
using static TD.Core;

namespace TD
{
    internal abstract class TryParser<Reduction, TInput> : DefaultCompletionReducer<Reduction, string, TInput?>
        where TInput : struct
    {
        public TryParser(IReducer<Reduction, TInput?> next) : base(next) { }

        public override Terminator<Reduction> Invoke(Reduction reduction, string value)
        {
            TInput x;
            if (TryParse(value, out x))
            {
                return Next.Invoke(reduction, x);
            }
            else
            {
                return Next.Invoke(reduction, null);
            }
        }

        protected abstract bool TryParse(string value, out TInput result);
    }

    internal class ByteTryParser<Reduction> : TryParser<Reduction, byte>
    {
        public ByteTryParser(IReducer<Reduction, byte?> next) : base(next) { }

        protected override bool TryParse(string value, out byte result) => byte.TryParse(value, out result);
    }

    internal class SByteTryParser<Reduction> : TryParser<Reduction, sbyte>
    {
        public SByteTryParser(IReducer<Reduction, sbyte?> next) : base(next) { }

        protected override bool TryParse(string value, out sbyte result) => sbyte.TryParse(value, out result);
    }

    internal class Int16TryParser<Reduction> : TryParser<Reduction, short>
    {
        public Int16TryParser(IReducer<Reduction, short?> next) : base(next) { }

        protected override bool TryParse(string value, out short result) => short.TryParse(value, out result);
    }

    internal class UInt16TryParser<Reduction> : TryParser<Reduction, ushort>
    {
        public UInt16TryParser(IReducer<Reduction, ushort?> next) : base(next) { }

        protected override bool TryParse(string value, out ushort result) => ushort.TryParse(value, out result);
    }

    internal class Int32TryParser<Reduction> : TryParser<Reduction, int>
    {
        public Int32TryParser(IReducer<Reduction, int?> next) : base(next) { }

        protected override bool TryParse(string value, out int result) => int.TryParse(value, out result);
    }

    internal class UInt32TryParser<Reduction> : TryParser<Reduction, uint>
    {
        public UInt32TryParser(IReducer<Reduction, uint?> next) : base(next) { }

        protected override bool TryParse(string value, out uint result) => uint.TryParse(value, out result);
    }

    internal class Int64TryParser<Reduction> : TryParser<Reduction, long>
    {
        public Int64TryParser(IReducer<Reduction, long?> next) : base(next) { }

        protected override bool TryParse(string value, out long result) => long.TryParse(value, out result);
    }

    internal class UInt64TryParser<Reduction> : TryParser<Reduction, ulong>
    {
        public UInt64TryParser(IReducer<Reduction, ulong?> next) : base(next) { }

        protected override bool TryParse(string value, out ulong result) => ulong.TryParse(value, out result);
    }

    internal class EnumTryParser<Reduction, TEnum> : TryParser<Reduction, TEnum> where TEnum : struct
    {
        public EnumTryParser(IReducer<Reduction, TEnum?> next) : base(next) { }

        protected override bool TryParse(string value, out TEnum result) => Enum.TryParse(value, out result);
    }

    internal class TryParsingTransducer<T> : ITransducer<string, T?> where T : struct
    {
        public IReducer<Reduction, string> Apply<Reduction>(IReducer<Reduction, T?> reducer)
        {
            if (typeof(T) == typeof(byte))
            {
                return new ByteTryParser<Reduction>((IReducer<Reduction, byte?>)reducer);
            }

            if (typeof(T) == typeof(sbyte))
            {
                return new SByteTryParser<Reduction>((IReducer<Reduction, sbyte?>)reducer);
            }

            if (typeof(T) == typeof(short))
            {
                return new Int16TryParser<Reduction>((IReducer<Reduction, short?>)reducer);
            }

            if (typeof(T) == typeof(ushort))
            {
                return new UInt16TryParser<Reduction>((IReducer<Reduction, ushort?>)reducer);
            }

            if (typeof(T) == typeof(int))
            {
                return new Int32TryParser<Reduction>((IReducer<Reduction, int?>)reducer);
            }

            if (typeof(T) == typeof(uint))
            {
                return new UInt32TryParser<Reduction>((IReducer<Reduction, uint?>)reducer);
            }

            if (typeof(T) == typeof(long))
            {
                return new Int64TryParser<Reduction>((IReducer<Reduction, long?>)reducer);
            }

            if (typeof(T) == typeof(ulong))
            {
                return new UInt64TryParser<Reduction>((IReducer<Reduction, ulong?>)reducer);
            }

            return new EnumTryParser<Reduction, T>(reducer);
        }
    }
    
    internal class ParsingTransducer<T> : ITransducer<string, T> where T : struct
    {
        private IReducer<Reduction, string> From<Reduction, U>(
            Func<string, U> func,
            IReducer<Reduction, T> reducer)
        {
            return Mapping(func).Apply((IReducer<Reduction, U>)reducer);
        }

        public IReducer<Reduction, string> Apply<Reduction>(IReducer<Reduction, T> reducer)
        {
            if (typeof(T) == typeof(byte))
            {
                return From(byte.Parse, reducer);
            }

            if (typeof(T) == typeof(sbyte))
            {
                return From(sbyte.Parse, reducer);
            }

            if (typeof(T) == typeof(short))
            {
                return From(short.Parse, reducer);
            }

            if (typeof(T) == typeof(ushort))
            {
                return From(ushort.Parse, reducer);
            }

            if (typeof(T) == typeof(int))
            {
                return From(int.Parse, reducer);
            }

            if (typeof(T) == typeof(uint))
            {
                return From(uint.Parse, reducer);
            }

            if (typeof(T) == typeof(long))
            {
                return From(long.Parse, reducer);
            }

            if (typeof(T) == typeof(ulong))
            {
                return From(ulong.Parse, reducer);
            }

            return From(value => (T)Enum.Parse(typeof(T), value), reducer);
        }
    }
}
