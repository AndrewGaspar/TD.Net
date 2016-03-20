using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TD;
using static TD.Terminator;

namespace TD.Test.Common
{
    public class VerifySequence<T> : IReducer<int, T>
    {
        private IList<T> sequence;

        public VerifySequence(IList<T> sequence)
        {
            this.sequence = sequence;
        }

        public Terminator<int> Complete(int reduction)
        {
            Assert.AreEqual(sequence.Count, reduction);
            return Termination(reduction);
        }

        public Terminator<int> Invoke(int index, T value)
        {
            Assert.AreEqual(sequence[index], value);
            return Reduction(index + 1);
        }
    }
}
