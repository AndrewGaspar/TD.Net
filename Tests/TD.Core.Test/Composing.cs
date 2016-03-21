using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using TD.Test.Common;
using static TD.Core;

namespace TD.Test
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
