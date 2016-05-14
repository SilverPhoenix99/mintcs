using System.Reflection;
using Mint.MethodBinding;
using Mint.MethodBinding.CallCompilation;
using NUnit.Framework;

namespace Mint.UnitTests
{
    [TestFixture]
    [TestOf(typeof(CallSite))]
    public class CallSiteTests
    {
        [Test]
        public void TestMonomorphicCall()
        {
            var callSite = GetClassCallSite();
            callSite.CallCompiler = new MonomorphicCallCompiler(callSite);
            
            Assert.That(callSite.Call(new Fixnum()), Is.EqualTo(Class.FIXNUM));

            Assert.That(callSite.CallInfo.ToString(), Is.Not.Null);
        }

        private static CallSite GetClassCallSite()
        {
            var methodName = new Symbol("class");
            var callInfo = new CallInfo(methodName);
            return new CallSite(callInfo);
        }

        [Test]
        public void TestPolymorphicCall()
        {
            var callSite = GetClassCallSite();
            callSite.CallCompiler = new PolymorphicCallCompiler(callSite);

            Assert.That(callSite.Call(new TrueClass()), Is.EqualTo(Class.TRUE));
            Assert.That(callSite.Call(new FalseClass()), Is.EqualTo(Class.FALSE));
            Assert.That(callSite.Call(new NilClass()), Is.EqualTo(Class.NIL));
            Assert.That(callSite.Call(new Fixnum()), Is.EqualTo(Class.FIXNUM));
        }

        [Test]
        public void TestMegamorphicCall()
        {
            var callSite = GetClassCallSite();
            callSite.CallCompiler = new MegamorphicCallCompiler(callSite);

            Assert.That(callSite.Call(new TrueClass()), Is.EqualTo(Class.TRUE));
            Assert.That(callSite.Call(new FalseClass()), Is.EqualTo(Class.FALSE));
            Assert.That(callSite.Call(new NilClass()), Is.EqualTo(Class.NIL));
            Assert.That(callSite.Call(new Fixnum()), Is.EqualTo(Class.FIXNUM));
        }

        [Test]
        public void TestDefaultCall()
        {
            var callSite = GetClassCallSite();
            Assert.That(callSite.Call(new TrueClass()), Is.EqualTo(Class.TRUE));
            Assert.That(callSite.Call(new FalseClass()), Is.EqualTo(Class.FALSE));
            Assert.That(callSite.Call(new NilClass()), Is.EqualTo(Class.NIL));
            Assert.That(callSite.Call(new Fixnum()), Is.EqualTo(Class.FIXNUM));
        }

        [Test]
        public void TestCallCompilerUpgrade()
        {
            var callSite = GetClassCallSite();
            Assert.That(callSite.CallCompiler, Is.Null);

            callSite.Call(new Fixnum());

            Assert.That(callSite.CallCompiler, Is.InstanceOf(typeof(PolymorphicCallCompiler)));

            var threshold = GetPolymorphicCacheThreshold();

            for(var i = 0; i < threshold + 1; i++)
            {
                var obj = new Object();
                var forcedSingletonClass = obj.SingletonClass;
                callSite.Call(obj);
            }

            Assert.That(callSite.CallCompiler, Is.InstanceOf(typeof(MegamorphicCallCompiler)));
        }

        private static int GetPolymorphicCacheThreshold()
        {
            return (int) typeof(PolymorphicCallCompiler).GetField(
                "CACHE_FULL_THRESHOLD",
                BindingFlags.NonPublic | BindingFlags.Static).GetRawConstantValue();
        }

        [Test]
        public void TestResultBoxing()
        {
            var callSite = new CallSite(new CallInfo(new Symbol("to_s")));

            const int integer = 100;
            var fixnum = new Fixnum(integer);
            var stringValue = new String(fixnum.Value.ToString());

            Assert.That(callSite.Call(fixnum), Is.EqualTo(stringValue));
        }
    }
}
