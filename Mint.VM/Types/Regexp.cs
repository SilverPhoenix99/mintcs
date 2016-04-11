using System;
using System.Reflection;

namespace Mint
{
    public class Regexp : BaseObject
    {
        public Regexp() : base(CLASS)
        {
            throw new NotImplementedException();
        }

        #region Static

        public static readonly Class CLASS;

        static Regexp()
        {
            CLASS = ClassBuilder<Regexp>.Describe();
        }

        #endregion
    }
}
