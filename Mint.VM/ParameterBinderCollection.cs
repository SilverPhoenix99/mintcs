using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Mint.MethodBinding.Parameters;

namespace Mint
{
    public class ParameterBinderCollection : Attribute
    {
        public ParameterBinderCollection(IList<ParameterBinder> binders)
        {
            Binders = binders.IsReadOnly ? binders : new ReadOnlyCollection<ParameterBinder>(binders);
        }


        public IList<ParameterBinder> Binders { get; }
    }
}