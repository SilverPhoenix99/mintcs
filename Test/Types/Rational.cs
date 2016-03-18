using System.Reflection;

namespace Mint
{
    public class Rational : Object
    {
        public static new readonly Class CLASS;

        // This class is a headache => do later

        static Rational()
        {
            CLASS = new Class(new Symbol(MethodBase.GetCurrentMethod().DeclaringType.Name));
        }
    }
}
