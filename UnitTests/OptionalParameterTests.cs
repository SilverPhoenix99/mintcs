using Mint.Binding;
using Mint.Binding.Arguments;
using Mint.Binding.Compilation;
using Mint.Reflection;
using Mint.Reflection.Parameters;
using NUnit.Framework;

namespace Mint.UnitTests
{
    [TestFixture]
    public class OptionalParameterTests
    {
        [Test]
        public void TestArity()
        {
            var binder = new String("").EffectiveClass.FindMethod(new Symbol("ljust"));

            Assert.That(binder.Arity, Is.EqualTo(new Arity(1, 2)));
        }

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
            var callSite = CallSite.Create(name, arguments);
            callSite.CallCompiler = new MonomorphicCallCompiler(callSite);
            return callSite;
        }
    }
}
