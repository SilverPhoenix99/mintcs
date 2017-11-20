using Mint.MethodBinding;
using Mint.MethodBinding.Arguments;
using NUnit.Framework;

namespace Mint.UnitTests
{
    [TestFixture]
    public class OptionalParameterTests
    {
        [Test]
        public void Test_String_ljust()
        {
            var instance = new String("hello");

            var callSite = CreateCallSite("ljust", ArgumentKind.Simple);
            Assert.That(callSite.Call(instance, new Fixnum(4)).ToString(), Is.EqualTo("hello"));
            Assert.That(callSite.Call(instance, new Fixnum(5)).ToString(), Is.EqualTo("hello"));
            Assert.That(callSite.Call(instance, new Fixnum(10)).ToString(), Is.EqualTo("hello     "));

            var length = new Fixnum(9);
            var padding = new String("1234");

            callSite = CreateCallSite("ljust", ArgumentKind.Simple, ArgumentKind.Simple);
            Assert.That(
                callSite.Call(instance, length, padding).ToString(),
                Is.EqualTo("hello1234")
            );

            length = new Fixnum(11);
            Assert.That(
                callSite.Call(instance, length, padding).ToString(),
                Is.EqualTo("hello123412")
            );
        }

        private static CallSite CreateCallSite(string methodName, params ArgumentKind[] arguments)
        {
            var name = new Symbol(methodName);
            var callSite = new CallSite(name, argumentKinds: arguments);
            return callSite;
        }
    }
}
