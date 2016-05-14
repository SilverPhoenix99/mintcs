using NUnit.Framework;

namespace Mint.UnitTests
{
    [TestFixture]
    [TestOf(typeof(Condition))]
    public class ConditionTests
    {
        [Test]
        public void TestCondition()
        {
            var cond = new Condition();

            Assert.IsTrue(cond.Valid);

            cond.Invalidate();

            Assert.IsFalse(cond.Valid);
        }
    }
}
