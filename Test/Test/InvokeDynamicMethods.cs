using System;
using Mint;

namespace Test
{
    class InvokeDynamicMethods
    {
        public static void Test()
        {
            Fixnum f = (Fixnum) 132;

            /*f.Class.DefineMethod(new Symbol("to_s"), (Func<Fixnum, Mint.String>)
                ( (self) => new Mint.String(self.ToString()) )
            );*/

            /*try
            {
                f.Class.Def<Fixnum>("to_n", "ToNative");
                throw new InvalidOperationException();
            }
            catch(NoMethodError) {}*/

            var d = (dynamic) f;

            //Console.WriteLine(d.to_s());
            Console.WriteLine(d.to_s);
            //Console.WriteLine(d.Class);
            Console.WriteLine(d.ToString());

            /*try
            {
                var n = ((dynamic) f).to_n();
                throw new InvalidOperationException();
            }
            catch(NoMethodError) {}*/
        }
    }
}
