using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TD.Test.Common;
using System.Collections.Generic;

namespace TD.Test
{
    internal static class Extensions
    {
        public static T CheckedAccumulate<T>(this IList<T> input) => 
            input.Reduce(default(T), Accumulator.Checked<T>()).Value;

        public static T UncheckedAccumulate<T>(this IList<T> input) =>
            input.Reduce(default(T), Accumulator.Unchecked<T>()).Value;
    }

    [TestClass]
    public class AccumulatorTests
    {
        [TestMethod]
        public void CheckedByte()
        {
            Verify.Throws<OverflowException>(() => new byte[] { 255, 1 }.CheckedAccumulate());
        }

        [TestMethod]
        public void UncheckedByte()
        {
            Assert.AreEqual<byte>(0, new byte[] { 255, 1 }.UncheckedAccumulate());
        }

        [TestMethod]
        public void CheckedSByte()
        {
            Verify.Throws<OverflowException>(() => new sbyte[] { 127, 1 }.CheckedAccumulate());
        }

        [TestMethod]
        public void UncheckedSByte()
        {
            Assert.AreEqual<sbyte>(-128, new sbyte[] { 127, 1 }.UncheckedAccumulate());
        }

        [TestMethod]
        public void CheckedShort()
        {
            Verify.Throws<OverflowException>(() => new short[] { 0x7FFF, 1 }.CheckedAccumulate());
        }

        [TestMethod]
        public void UncheckedShort()
        {
            Assert.AreEqual<short>(-0x8000, new short[] { 0x7FFF, 1 }.UncheckedAccumulate());
        }

        [TestMethod]
        public void CheckedUShort()
        {
            Verify.Throws<OverflowException>(() => new ushort[] { 0xFFFF, 1 }.CheckedAccumulate());
        }

        [TestMethod]
        public void UncheckedUShort()
        {
            Assert.AreEqual<ushort>(0, new ushort[] { 0xFFFF, 1 }.UncheckedAccumulate());
        }

        [TestMethod]
        public void CheckedInt()
        {
            Verify.Throws<OverflowException>(() => new int[] { 0x7FFFFFFF, 1 }.CheckedAccumulate());
        }

        [TestMethod]
        public void UncheckedInt()
        {
            Assert.AreEqual(-0x80000000, new int[] { 0x7FFFFFFF, 1 }.UncheckedAccumulate());
        }

        [TestMethod]
        public void CheckedUInt()
        {
            Verify.Throws<OverflowException>(() => new uint[] { 0xFFFFFFFF, 1 }.CheckedAccumulate());
        }

        [TestMethod]
        public void UncheckedUInt()
        {
            Assert.AreEqual<uint>(0, new uint[] { 0xFFFFFFFF, 1 }.UncheckedAccumulate());
        }

        [TestMethod]
        public void CheckedLong()
        {
            Verify.Throws<OverflowException>(() => new long[] { 0x7FFFFFFFFFFFFFFF, 1 }.CheckedAccumulate());
        }

        [TestMethod]
        public void UncheckedLong()
        {
            Assert.AreEqual(-0x8000000000000000, new long[] { 0x7FFFFFFFFFFFFFFF, 1 }.UncheckedAccumulate());
        }

        [TestMethod]
        public void CheckedULong()
        {
            Verify.Throws<OverflowException>(() => new ulong[] { 0xFFFFFFFFFFFFFFFF, 1 }.CheckedAccumulate());
        }

        [TestMethod]
        public void UncheckedULong()
        {
            Assert.AreEqual<ulong>(0, new ulong[] { 0xFFFFFFFFFFFFFFFF, 1 }.UncheckedAccumulate());
        }
    }
}
