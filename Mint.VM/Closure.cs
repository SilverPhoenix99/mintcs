using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Mint.Reflection;
using Mint.MethodBinding.Methods;

namespace Mint
{
    public class Closure
    {
        private List<LocalVariable> locals;

        public iObject Self => this[0];

        public Closure Parent { get; }

        public IEnumerable<Symbol> Names => locals.Skip(1).Select(local => local.Name); // skip "self"

        public Closure(iObject self, Closure parent = null)
        {
            Parent = parent;
            locals = new List<LocalVariable>();
            AddLocal(Symbol.SELF, self);
        }

        public iObject this[Symbol name]
        {
            get { return locals[IndexOf(name)].Value; }
            set { locals[IndexOf(name)].Value = value; }
        }

        public iObject this[int index]
        {
            get { return locals[index].Value; }
            set { locals[index].Value = value ?? new NilClass(); }
        }

        public bool IsDefined(Symbol name) => IndexOf(name) > -1;

        public int IndexOf(Symbol name) => locals.FindIndex(local => local.Name == name);

        public iObject AddLocal(Symbol name, iObject value = null)
        {
            if(value == null)
            {
                value = new NilClass();
            }

            locals.Add(new LocalVariable(name, value));
            return value;
        }

        public static class Reflection
        {
            public static readonly PropertyInfo Indexer = Reflector<Closure>.Property(_ => _[default(int)]);

            public static readonly PropertyInfo Self = Reflector<Closure>.Property(_ => _.Self);

            public static readonly PropertyInfo Parent = Reflector<Closure>.Property(_ => _.Parent);

            public static readonly MethodInfo AddLocal =
                Reflector<Closure>.Method(_ => _.AddLocal(default(Symbol), default(iObject)));
        }

        public static class Expressions
        {
            public static IndexExpression Indexer(Expression closure, Expression index) =>
                closure.Indexer(Reflection.Indexer, index);

            public static MemberExpression Self(Expression closure) =>
                Expression.Property(closure, Reflection.Self);

            public static MemberExpression Parent(Expression closure) =>
                Expression.Property(closure, Reflection.Parent);

            public static MethodCallExpression AddLocal(Expression closure, Expression name, Expression value) =>
                Expression.Call(closure, Reflection.AddLocal, name, value);
        }
    }
}
