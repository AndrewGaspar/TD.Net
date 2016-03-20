using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using TD.Test.Common;
using static TD.Transducer;

namespace TD.Core.Test
{
    [TestClass]
    public class ComposingTests
    {
        [TestMethod]
        public void TakingDoubleDigit()
        {
            Verify.SequenceEquivalent(
                Enumerable.Range(0, 15),
                new[] { "10", "11", "12", "13", "14" },
                Passing<int>()
                    .Mapping(x => x.ToString())
                    .Filtering(str => str.Length == 2));
        }
    }
}
