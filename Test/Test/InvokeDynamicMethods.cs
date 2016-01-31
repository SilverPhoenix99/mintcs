using System;
using Mint.Types;
using Mint;

namespace Test
{
    class InvokeDynamicMethods
    {
        public static void Test()
        {
            Fixnum f = (Fixnum) 132;

            f.Class.Def<Fixnum>("to_s", "ToString");

            try
            {
                f.Class.Def<Fixnum>("to_n", "ToNative");
                throw new InvalidOperationException();
            }
            catch(NoMethodError) {}

            var s = ((dynamic) f).to_s();


            var s2 = ((dynamic) f).ToString();

            try
            {
                var n = ((dynamic) f).to_n();
                throw new InvalidOperationException();
            }
            catch(NoMethodError) {}
        }
    }
}
