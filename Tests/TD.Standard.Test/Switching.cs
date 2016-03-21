using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using TD.Test.Common;
using static TD.Core;
using static TD.Standard;

namespace TD.Test
{
    [TestClass]
    public class SwitchingTests
    {
        enum Color
        {
            Red,
            Blue
        }

        [TestMethod]
        public void Coloring()
        {
            Verify.SequenceEquivalent(
                Enumerable.Range(0, 5),
                new[] { Color.Blue, Color.Red, Color.Blue, Color.Red, Color.Blue },
                Switching(
                    TransducerSwitch.Create(x => x % 2 == 0, Mapping<int, Color>(_ => Color.Blue)),
                    TransducerSwitch.Default(Mapping<int, Color>(_ => Color.Red))));
        }
    }
}
