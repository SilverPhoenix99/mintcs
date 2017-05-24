using System.Reflection;
using Mint.Reflection;

namespace Mint
{
    public class Condition
    {
        public bool Valid { get; private set; } = true;


        public void Invalidate()
        {
            Valid = false;
        }


        public static class Reflection
        {
            public static readonly PropertyInfo Valid = Reflector<Condition>.Property(_ => _.Valid);
        }
    }
}
