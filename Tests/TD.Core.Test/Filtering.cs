using Microsoft.VisualStudio.TestTools.UnitTesting;
using TD.Test.Common;
using System.Linq;

namespace TD.Test
{
    [TestClass]
    public class FilteringTests
    {
        [TestMethod]
        public void EvenOnly()
        {
            Verify.SequenceEquivalent(
                Enumerable.Range(0, 5),
                new[] { 0, 2, 4 },
                Core.Filtering<int>(x => x % 2 == 0));
        }

        [TestMethod]
        public void OddOnly()
        {
            Verify.SequenceEquivalent(
                Enumerable.Range(0, 5),
                new[] { 1, 3 },
                Core.Filtering<int>(x => x % 2 == 1));
        }
    }
}
