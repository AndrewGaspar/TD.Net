using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using TD.Test.Common;

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
                Transducer.Mapping<int, string>(x => x.ToString())
                          .Compose(Transducer.Filtering<string>(str => str.Length == 2)));
        }
    }
}
