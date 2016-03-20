using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using TD.Test.Common;

namespace TD.Core.Test
{
    [TestClass]
    public class MappingTests
    {
        [TestMethod]
        public void IncrementsValues()
        {
            Verify.SequenceEquivalent(
                Enumerable.Range(0, 5),
                Enumerable.Range(1, 5).ToArray(),
                Transducer.Mapping<int, int>(x => x + 1));
        }

        [TestMethod]
        public void DecrementsValue()
        {
            Verify.SequenceEquivalent(
                Enumerable.Range(0, 5),
                Enumerable.Range(-1, 5).ToArray(),
                Transducer.Mapping<int, int>(x => x - 1));
        }

        [TestMethod]
        public void StringifiesValues()
        {
            Verify.SequenceEquivalent(
                Enumerable.Range(0, 5),
                new[] { "0", "1", "2", "3", "4" },
                Transducer.Mapping<int, string>(x => x.ToString()));
        }
    }
}
