using Mint.MethodBinding;
using Mint.MethodBinding.Cache;
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
            callSite.CallCache = new MonomorphicCallSiteCache(
                callSite,
                Class.FIXNUM.Id,
                Class.FIXNUM.FindMethod(new Symbol("class"))
            );

            Assert.That(callSite.Call(new Fixnum()), Is.EqualTo(Class.FIXNUM));

            Assert.That(callSite.Visibility, Is.EqualTo(Visibility.Public));
            Assert.That(callSite.ToString(), Is.Not.Null);
        }

        private static CallSite GetClassCallSite()
        {
            return new CallSite(new Symbol("class"));
        }

        [Test]
        public void TestPolymorphicCall()
        {
            var callSite = GetClassCallSite();
            callSite.CallCache = new PolymorphicCallSiteCache(callSite);

            Assert.That(callSite.Call(new TrueClass()), Is.EqualTo(Class.TRUE));
            Assert.That(callSite.Call(new FalseClass()), Is.EqualTo(Class.FALSE));
            Assert.That(callSite.Call(new NilClass()), Is.EqualTo(Class.NIL));
            Assert.That(callSite.Call(new Fixnum()), Is.EqualTo(Class.FIXNUM));
        }

        [Test]
        public void TestMegamorphicCall()
        {
            var callSite = GetClassCallSite();
            callSite.CallCache = new MegamorphicCallSiteCache(callSite);

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
            callSite.CallCache = new PolymorphicCallSiteCache(callSite);

            for(var i = 0; i < PolymorphicCallSiteCache.MAX_CACHE_THRESHOLD + 1; i++)
            {
                var obj = new Object();
                var forcedSingletonClass = obj.SingletonClass;
                callSite.Call(obj);
            }

            Assert.That(callSite.CallCache, Is.InstanceOf(typeof(MegamorphicCallSiteCache)));
        }
        
        [Test]
        public void TestResultBoxing()
        {
            var callSite = new CallSite(new Symbol("to_s"));

            const int integer = 100;
            var fixnum = new Fixnum(integer);
            var stringValue = new String(fixnum.Value.ToString());

            Assert.That(callSite.Call(fixnum), Is.EqualTo(stringValue));
        }
    }
}
