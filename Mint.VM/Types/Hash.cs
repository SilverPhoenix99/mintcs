using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Mint.MethodBinding;
using Mint.Reflection;
using Mint.Reflection.Parameters.Attributes;

namespace Mint
{
    /*
        ::methods:
          []  try_convert
        #methods:
          initialize       assoc                 delete      fetch_values  key      reject!  to_proc
          initialize_copy  clear                 delete_if   flatten       key?     replace  to_s
          <                compact               dig         has_key?      keys     select   transform_values
          <=               compact!              each        has_value?    length   select!  transform_values!
          ==               compare_by_identity   each_key    hash          member?  shift    update
          >                compare_by_identity?  each_pair   include?      merge    size     value?
          >=               default               each_value  index         merge!   store    values
          []               default=              empty?      inspect       rassoc   to_a     values_at
          []=              default_proc          eql?        invert        rehash   to_h
          any?             default_proc=         fetch       keep_if       reject   to_hash
    */

    [RubyClass]
    public class Hash : BaseObject, IEnumerable<iObject>
    {
        private readonly LinkedDictionary<iObject, iObject> map;
        
        public Hash() : base(Class.HASH)
        {
            map = new LinkedDictionary<iObject, iObject>();
        }

        public Hash(int capacity) : base(Class.HASH)
        {
            map = new LinkedDictionary<iObject, iObject>(capacity);
        }
        
        internal Hash(IDictionary<iObject, iObject> map) : this(map.Count)
        {
            foreach(var pair in map)
            {
                this[pair.Key] = pair.Value;
            }
        }
        
        internal Hash(Hash other) : this(other.map)
        { }

        [RubyMethod("keys")]
        public IEnumerable<iObject> Keys => map.Keys;

        [RubyMethod("values")]
        public IEnumerable<iObject> Values => map.Values;

        [RubyMethod("length")]
        [RubyMethod("size")]
        public int Count => map.Count;

        public iObject this[iObject key]
        {
            [RubyMethod("[]")]
            get
            {
                map.TryGetValue(key, out var value);
                return value;
            }

            [RubyMethod("[]=")]
            set => map[key] = value;
        }
        
        public Array ToArray() => new Array(map.Select(_ => new Array(_.Key, _.Value)));

        public IEnumerator<iObject> GetEnumerator()
            => map.Select(_ => (iObject) new Array(_.Key, _.Value)).GetEnumerator();
        
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        [RubyMethod("inspect")]
        [RubyMethod("to_s")]
        public override string ToString()
        {
            var elements = map.Select(_ => $"{_.Key.Inspect()}=>{_.Value.Inspect()}");
            return $"{{{string.Join(", ", elements)}}}";
        }

        [RubyMethod("merge")]
        public Hash MergeSelf(Hash otherHash)
        {
            foreach(var element in otherHash.map)
            {
                map.Add(element);
            }

            return this;
        }

        [RubyMethod("merge!")]
        public Hash Merge(Hash otherHash) => new Hash(map).MergeSelf(otherHash);

        public bool HasKey(iObject key) => map.ContainsKey(key);
        
        public Hash Duplicate() => new Hash(this);

        [RubyMethod("initialize", Visibility = Visibility.Private)]
        [RubyMethod("initialize_copy", Visibility = Visibility.Private)]
        [RubyMethod("<")]
        [RubyMethod("<=")]
        [RubyMethod("==")]
        [RubyMethod(">")]
        [RubyMethod(">=")]
        [RubyMethod("any?")]
        [RubyMethod("assoc")]
        [RubyMethod("clear")]
        [RubyMethod("compact")]
        [RubyMethod("compact!")]
        [RubyMethod("compare_by_identity")]
        [RubyMethod("compare_by_identity?")]
        [RubyMethod("default")]
        [RubyMethod("default=")]
        [RubyMethod("default_proc")]
        [RubyMethod("default_proc=")]
        [RubyMethod("delete")]
        [RubyMethod("delete_if")]
        [RubyMethod("dig")]
        [RubyMethod("each")]
        [RubyMethod("each_key")]
        [RubyMethod("each_pair")]
        [RubyMethod("each_value")]
        [RubyMethod("empty?")]
        [RubyMethod("eql?")]
        [RubyMethod("fetch")]
        [RubyMethod("fetch_values")]
        [RubyMethod("flatten")]
        [RubyMethod("has_key?")]
        [RubyMethod("has_value?")]
        [RubyMethod("hash")]
        [RubyMethod("include?")]
        [RubyMethod("index")]
        [RubyMethod("invert")]
        [RubyMethod("keep_if")]
        [RubyMethod("key")]
        [RubyMethod("key?")]
        [RubyMethod("member?")]
        [RubyMethod("rassoc")]
        [RubyMethod("rehash")]
        [RubyMethod("reject")]
        [RubyMethod("reject!")]
        [RubyMethod("replace")]
        [RubyMethod("select")]
        [RubyMethod("select!")]
        [RubyMethod("shift")]
        [RubyMethod("store")]
        [RubyMethod("to_a")]
        [RubyMethod("to_h")]
        [RubyMethod("to_hash")]
        [RubyMethod("to_proc")]
        [RubyMethod("transform_values")]
        [RubyMethod("transform_values!")]
        [RubyMethod("update")]
        [RubyMethod("value?")]
        [RubyMethod("values_at")]
        public void NotImplemented([Rest] Array args, [Block] object block)
            => throw new NotImplementedException(
                $"{nameof(Hash)}#{CallFrame.Current.CallSite.MethodName.Name}"
            );

        internal static ModuleBuilder<Hash> Build() =>
            ModuleBuilder<Hash>.DescribeClass()
                .GenerateAllocator()
                .AutoDefineMethods()
        ;

        public static class Reflection
        {
            public static readonly ConstructorInfo Ctor = Reflector.Ctor<Hash>();

            public static readonly MethodInfo MergeSelf = Reflector<Hash>.Method(_ => _.MergeSelf(default));

            public static readonly PropertyInfo Indexer = Reflector<Hash>.Property(_ => _[default]);
        }
        
        public static class Expressions
        {
            public static NewExpression New() => Expression.New(Reflection.Ctor);
            
            public static MethodCallExpression MergeSelf(Expression hash, Expression otherHash)
                => Expression.Call(hash, Reflection.MergeSelf, otherHash);
            
            public static IndexExpression Indexer(Expression hash, Expression key)
                => Expression.Property(hash, Reflection.Indexer, key);
        }
    }
}