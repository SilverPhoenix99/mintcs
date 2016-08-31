using System.Reflection;
using Mint.Reflection;
using Mint.Reflection.Parameters;
using Mint.Reflection.Parameters.Attributes;
using NUnit.Framework;

namespace Mint.UnitTests
{
    [TestFixture]
    public class ParameterKindTests
    {
        private static void DummyMethod(
                             iObject prefixRequired,
            [Optional]       iObject optional,
            [Rest]           iObject rest,
                             iObject suffixRequired,
            [Key]            iObject keyRequired1,
            [Key] [Optional] iObject keyOptional1,
            [Key]            iObject keyRequired2,
            [Key] [Optional] iObject keyOptional2,
            [Key] [Rest]     iObject keyRest,
            [Block]          iObject block
        )
        { }

        private static readonly MethodInfo DUMMYMETHOD_INFO = Reflector.Method(
            () => DummyMethod(null, null, null, null, null, null, null, null, null, null)
        );

        [Test]
        public void TestRequiredParameter()
        {
            var parameter = DUMMYMETHOD_INFO.GetParameters()[0];

            Assert.IsFalse(parameter.IsOptional());
            Assert.IsFalse(parameter.IsRest());
            Assert.IsFalse(parameter.IsKey());
            Assert.IsFalse(parameter.IsKeyRest());
            Assert.IsFalse(parameter.IsBlock());
            Assert.IsTrue(parameter.IsRequired());
        }

        [Test]
        public void TestOptionalParameter()
        {
            var parameter = DUMMYMETHOD_INFO.GetParameters()[1];

            Assert.IsTrue(parameter.IsOptional());
            Assert.IsFalse(parameter.IsRest());
            Assert.IsFalse(parameter.IsKey());
            Assert.IsFalse(parameter.IsKeyRest());
            Assert.IsFalse(parameter.IsBlock());
            Assert.IsFalse(parameter.IsRequired());
        }

        [Test]
        public void TestRestParameter()
        {
            var parameter = DUMMYMETHOD_INFO.GetParameters()[2];

            Assert.IsFalse(parameter.IsOptional());
            Assert.IsTrue(parameter.IsRest());
            Assert.IsFalse(parameter.IsKey());
            Assert.IsFalse(parameter.IsKeyRest());
            Assert.IsFalse(parameter.IsBlock());
            Assert.IsFalse(parameter.IsRequired());
        }

        [Test]
        public void TestKeyRequiredParameter()
        {
            var parameter = DUMMYMETHOD_INFO.GetParameters()[4];

            Assert.IsFalse(parameter.IsOptional());
            Assert.IsFalse(parameter.IsRest());
            Assert.IsTrue(parameter.IsKey());
            Assert.IsFalse(parameter.IsKeyRest());
            Assert.IsFalse(parameter.IsBlock());
            Assert.IsTrue(parameter.IsRequired());
        }

        [Test]
        public void TestKeyOptionalParameter()
        {
            var parameter = DUMMYMETHOD_INFO.GetParameters()[5];

            Assert.IsTrue(parameter.IsOptional());
            Assert.IsFalse(parameter.IsRest());
            Assert.IsTrue(parameter.IsKey());
            Assert.IsFalse(parameter.IsKeyRest());
            Assert.IsFalse(parameter.IsBlock());
            Assert.IsFalse(parameter.IsRequired());
        }

        [Test]
        public void TestKeyRestParameter()
        {
            var parameter = DUMMYMETHOD_INFO.GetParameters()[8];

            Assert.IsFalse(parameter.IsOptional());
            Assert.IsTrue(parameter.IsRest());
            Assert.IsTrue(parameter.IsKey());
            Assert.IsTrue(parameter.IsKeyRest());
            Assert.IsFalse(parameter.IsBlock());
            Assert.IsFalse(parameter.IsRequired());
        }

        [Test]
        public void TestBlockParameter()
        {
            var parameter = DUMMYMETHOD_INFO.GetParameters()[9];

            Assert.IsFalse(parameter.IsOptional());
            Assert.IsFalse(parameter.IsRest());
            Assert.IsFalse(parameter.IsKey());
            Assert.IsFalse(parameter.IsKeyRest());
            Assert.IsTrue(parameter.IsBlock());
            Assert.IsFalse(parameter.IsRequired());
        }

        [Test]
        public void TestGetParameterKind()
        {
            var parameters = DUMMYMETHOD_INFO.GetParameters();

            Assert.That(parameters[0].GetParameterKind(), Is.EqualTo(ParameterKind.Required));
            Assert.That(parameters[1].GetParameterKind(), Is.EqualTo(ParameterKind.Optional));
            Assert.That(parameters[2].GetParameterKind(), Is.EqualTo(ParameterKind.Rest));
            Assert.That(parameters[3].GetParameterKind(), Is.EqualTo(ParameterKind.Required));
            Assert.That(parameters[4].GetParameterKind(), Is.EqualTo(ParameterKind.KeyRequired));
            Assert.That(parameters[5].GetParameterKind(), Is.EqualTo(ParameterKind.KeyOptional));
            Assert.That(parameters[6].GetParameterKind(), Is.EqualTo(ParameterKind.KeyRequired));
            Assert.That(parameters[7].GetParameterKind(), Is.EqualTo(ParameterKind.KeyOptional));
            Assert.That(parameters[8].GetParameterKind(), Is.EqualTo(ParameterKind.KeyRest));
            Assert.That(parameters[9].GetParameterKind(), Is.EqualTo(ParameterKind.Block));
        }

        [Test]
        public void TestParameterInformation()
        {
            ParameterCounter parameters = null;

            Assert.DoesNotThrow(() => { parameters = new ParameterCounter(DUMMYMETHOD_INFO); });

            Assert.That(parameters.PrefixRequired, Is.EqualTo(1));
            Assert.That(parameters.Optional, Is.EqualTo(1));
            Assert.IsTrue(parameters.HasRest);
            Assert.That(parameters.SuffixRequired, Is.EqualTo(1));
            Assert.That(parameters.KeyRequired, Is.EqualTo(2));
            Assert.That(parameters.KeyOptional, Is.EqualTo(2));
            Assert.IsTrue(parameters.HasKeyRest);
            Assert.IsTrue(parameters.HasBlock);
        }
    }
}
