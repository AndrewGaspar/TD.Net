using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace TD.Test.Common
{
    public static class Verify
    {
        public static void SequenceEquivalent<T, U>(IEnumerable<T> input, IList<U> result, ITransducer<T, U> transducer)
        {
            var termination = input.Reduce(0, transducer.Apply(new VerifySequence<U>(result)));

            Assert.IsTrue(termination.IsTerminated);
        }

        public static void Throws<TException>(Action action) where TException : Exception
        {
            var threw = false;

            try
            {
                action();
            }
            catch (TException)
            {
                threw = true;
            }

            Assert.IsTrue(threw);
        }
    }
}
