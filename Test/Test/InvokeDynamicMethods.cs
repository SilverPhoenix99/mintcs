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

            /*try
            {
                f.Class.Def<Fixnum>("to_n", "ToNative");
                throw new InvalidOperationException();
            }
            catch(NoMethodError) {}*/

            var d = (dynamic) f;

            var s = d.to_s();
            s = d.to_s;
            s = d.Class;
            s = d.ToString();

            /*try
            {
                var n = ((dynamic) f).to_n();
                throw new InvalidOperationException();
            }
            catch(NoMethodError) {}*/
        }
    }
}
