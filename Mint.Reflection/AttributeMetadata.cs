using System;
using System.Collections.Generic;
using System.Linq;

namespace Mint.Reflection
{
    public abstract class AttributeMetadata
    {
        protected AttributeMetadata(params IEnumerable<Attribute>[] attributesCollection)
        {
            Attributes = attributesCollection.Where(a => a != null).SelectMany(a => a).ToList();
        }


        public IList<Attribute> Attributes { get; }
    }
}
