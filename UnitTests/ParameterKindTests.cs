using System;
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
                             iObject instance,
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
        {
            Console.WriteLine(instance);
            Console.WriteLine(prefixRequired);
            Console.WriteLine(optional);
            Console.WriteLine(rest);
            Console.WriteLine(suffixRequired);
            Console.WriteLine(keyRequired1);
            Console.WriteLine(keyOptional1);
            Console.WriteLine(keyRequired2);
            Console.WriteLine(keyOptional2);
            Console.WriteLine(keyRest);
            Console.WriteLine(block);
        }

        private static readonly MethodMetadata DUMMY_METHOD_METADATA = new MethodMetadata(
            Reflector.Method(() => DummyMethod(null, null, null, null, null, null, null, null, null, null, null)),
            hasInstance: true
        );

        [Test]
        public void TestRequiredParameter()
        {
            var parameter = DUMMY_METHOD_METADATA.Parameters[0];

            Assert.IsFalse(parameter.IsOptional);
            Assert.IsFalse(parameter.IsRest);
            Assert.IsFalse(parameter.IsKey);
            Assert.IsFalse(parameter.IsKeyRest);
            Assert.IsFalse(parameter.IsBlock);
            Assert.IsTrue(parameter.IsRequired);
        }

        [Test]
        public void TestOptionalParameter()
        {
            var parameter = DUMMY_METHOD_METADATA.Parameters[1];

            Assert.IsTrue(parameter.IsOptional);
            Assert.IsFalse(parameter.IsRest);
            Assert.IsFalse(parameter.IsKey);
            Assert.IsFalse(parameter.IsKeyRest);
            Assert.IsFalse(parameter.IsBlock);
            Assert.IsFalse(parameter.IsRequired);
        }

        [Test]
        public void TestRestParameter()
        {
            var parameter = DUMMY_METHOD_METADATA.Parameters[2];

            Assert.IsFalse(parameter.IsOptional);
            Assert.IsTrue(parameter.IsRest);
            Assert.IsFalse(parameter.IsKey);
            Assert.IsFalse(parameter.IsKeyRest);
            Assert.IsFalse(parameter.IsBlock);
            Assert.IsFalse(parameter.IsRequired);
        }

        [Test]
        public void TestKeyRequiredParameter()
        {
            var parameter = DUMMY_METHOD_METADATA.Parameters[4];

            Assert.IsFalse(parameter.IsOptional);
            Assert.IsFalse(parameter.IsRest);
            Assert.IsTrue(parameter.IsKey);
            Assert.IsFalse(parameter.IsKeyRest);
            Assert.IsFalse(parameter.IsBlock);
            Assert.IsTrue(parameter.IsRequired);
        }

        [Test]
        public void TestKeyOptionalParameter()
        {
            var parameter = DUMMY_METHOD_METADATA.Parameters[5];

            Assert.IsTrue(parameter.IsOptional);
            Assert.IsFalse(parameter.IsRest);
            Assert.IsTrue(parameter.IsKey);
            Assert.IsFalse(parameter.IsKeyRest);
            Assert.IsFalse(parameter.IsBlock);
            Assert.IsFalse(parameter.IsRequired);
        }

        [Test]
        public void TestKeyRestParameter()
        {
            var parameter = DUMMY_METHOD_METADATA.Parameters[8];

            Assert.IsFalse(parameter.IsOptional);
            Assert.IsTrue(parameter.IsRest);
            Assert.IsTrue(parameter.IsKey);
            Assert.IsTrue(parameter.IsKeyRest);
            Assert.IsFalse(parameter.IsBlock);
            Assert.IsFalse(parameter.IsRequired);
        }

        [Test]
        public void TestBlockParameter()
        {
            var parameter = DUMMY_METHOD_METADATA.Parameters[9];

            Assert.IsFalse(parameter.IsOptional);
            Assert.IsFalse(parameter.IsRest);
            Assert.IsFalse(parameter.IsKey);
            Assert.IsFalse(parameter.IsKeyRest);
            Assert.IsTrue(parameter.IsBlock);
            Assert.IsFalse(parameter.IsRequired);
        }

        [Test]
        public void TestGetParameterKind()
        {
            var parameters = DUMMY_METHOD_METADATA.Parameters;

            Assert.That(parameters[0].Kind, Is.EqualTo(ParameterKind.Required));
            Assert.That(parameters[1].Kind, Is.EqualTo(ParameterKind.Optional));
            Assert.That(parameters[2].Kind, Is.EqualTo(ParameterKind.Rest));
            Assert.That(parameters[3].Kind, Is.EqualTo(ParameterKind.Required));
            Assert.That(parameters[4].Kind, Is.EqualTo(ParameterKind.KeyRequired));
            Assert.That(parameters[5].Kind, Is.EqualTo(ParameterKind.KeyOptional));
            Assert.That(parameters[6].Kind, Is.EqualTo(ParameterKind.KeyRequired));
            Assert.That(parameters[7].Kind, Is.EqualTo(ParameterKind.KeyOptional));
            Assert.That(parameters[8].Kind, Is.EqualTo(ParameterKind.KeyRest));
            Assert.That(parameters[9].Kind, Is.EqualTo(ParameterKind.Block));
        }

        [Test]
        public void TestParameterInformation()
        {
            ParameterCounter parameters = null;

            Assert.DoesNotThrow(() => { parameters = DUMMY_METHOD_METADATA.ParameterCounter; });

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
