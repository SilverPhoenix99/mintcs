using System.Reflection;

namespace Mint
{
    public class Regexp : Object
    {
        public static new readonly Class CLASS;

        static Regexp()
        {
            CLASS = new Class(new Symbol(MethodBase.GetCurrentMethod().DeclaringType.Name));
        }
    }
}
