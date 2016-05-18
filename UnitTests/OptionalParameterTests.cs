using Mint.MethodBinding;
using Mint.MethodBinding.CallCompilation;
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

            Assert.That(binder.Arity, Is.EqualTo(new Range(new Fixnum(1), new Fixnum(2))));
        }

        [Test]
        public void Test_String_ljust()
        {
            var instance = new String("hello");
            var callSite = CreateCallSite("ljust", ParameterKind.Required);

            Assert.That(
                callSite.Call(instance, new Fixnum(10), new String("1234")).ToString(),
                Is.EqualTo("hello12341")
            );

            //Assert.That(callSite.Call(instance, new Fixnum(4)).ToString(), Is.EqualTo("hello"));
            //Assert.That(callSite.Call(instance, new Fixnum(10)).ToString(), Is.EqualTo("hello     "));
        }

        private static CallSite CreateCallSite(string methodName, params ParameterKind[] parameters)
        {
            var name = new Symbol(methodName);
            var callInfo = new CallInfo(name, parameters: parameters);
            var callSite = new CallSite(callInfo);
            callSite.CallCompiler = new MonomorphicCallCompiler(callSite);
            return callSite;
        }
    }
}
