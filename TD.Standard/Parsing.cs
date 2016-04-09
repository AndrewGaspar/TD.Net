using System;
using static TD.Core;

namespace TD
{
    internal abstract class TryParser<TValue>
        where TValue : struct
    {
        public TValue? TryParse(string value)
        {
            TValue x;
            if(TryParseOut(value, out x))
            {
                return x;
            }

            return null;
        }

        public abstract bool TryParseOut(string value, out TValue result);

        public abstract TValue Parse(string value);

        public TryParser<TCast> As<TCast>() where TCast : struct => this as TryParser<TCast>;
    }

    internal class ByteParser : TryParser<byte>
    {
        public override byte Parse(string value) => byte.Parse(value);

        public override bool TryParseOut(string value, out byte result) => byte.TryParse(value, out result);
    }

    internal class SByteParser : TryParser<sbyte>
    {
        public override sbyte Parse(string value) => sbyte.Parse(value);

        public override bool TryParseOut(string value, out sbyte result) => sbyte.TryParse(value, out result);
    }

    internal class Int16Parser : TryParser<short>
    {
        public override short Parse(string value) => short.Parse(value);

        public override bool TryParseOut(string value, out short result) => short.TryParse(value, out result);
    }

    internal class UInt16Parser : TryParser<ushort>
    {
        public override ushort Parse(string value) => ushort.Parse(value);

        public override bool TryParseOut(string value, out ushort result) => ushort.TryParse(value, out result);
    }

    internal class Int32Parser : TryParser<int>
    {
        public override int Parse(string value) => int.Parse(value);

        public override bool TryParseOut(string value, out int result) => int.TryParse(value, out result);
    }

    internal class UInt32Parser : TryParser<uint>
    {
        public override uint Parse(string value) => uint.Parse(value);

        public override bool TryParseOut(string value, out uint result) => uint.TryParse(value, out result);
    }

    internal class Int64Parser : TryParser<long>
    {
        public override long Parse(string value) => long.Parse(value);

        public override bool TryParseOut(string value, out long result) => long.TryParse(value, out result);
    }

    internal class UInt64Parser : TryParser<ulong>
    {
        public override ulong Parse(string value) => ulong.Parse(value);

        public override bool TryParseOut(string value, out ulong result) => ulong.TryParse(value, out result);
    }

    internal class EnumParser<TEnum> : TryParser<TEnum> where TEnum : struct
    {
        public override TEnum Parse(string value) => (TEnum)Enum.Parse(typeof(TEnum), value);

        public override bool TryParseOut(string value, out TEnum result) => Enum.TryParse(value, out result);
    }

    internal static class Parsing
    {
        public static TryParser<T> GetParser<T>() where T : struct
        {
            if (typeof(T) == typeof(byte))
            {
                return new ByteParser().As<T>();
            }

            if (typeof(T) == typeof(sbyte))
            {
                return new SByteParser().As<T>();
            }

            if (typeof(T) == typeof(short))
            {
                return new Int16Parser().As<T>();
            }

            if (typeof(T) == typeof(ushort))
            {
                return new UInt16Parser().As<T>();
            }

            if (typeof(T) == typeof(int))
            {
                return new Int32Parser().As<T>();
            }

            if (typeof(T) == typeof(uint))
            {
                return new UInt32Parser().As<T>();
            }

            if (typeof(T) == typeof(long))
            {
                return new Int64Parser().As<T>();
            }

            if (typeof(T) == typeof(ulong))
            {
                return new UInt64Parser().As<T>();
            }

            return new EnumParser<T>();
        }
    }
}
