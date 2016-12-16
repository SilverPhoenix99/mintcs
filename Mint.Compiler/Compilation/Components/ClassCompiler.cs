using System.Linq.Expressions;
using System.Reflection;
using Mint.Compilation.Scopes;
using Mint.Parse;
using Mint.Reflection;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal abstract class ClassCompiler : CompilerComponentBase
    {
		protected virtual Ast<Token> NameNode => Node[0];

        protected Ast<Token> SuperclassNode => HasSuperclass ? Node[1] : null;

        protected bool HasSuperclass => !Node[1].IsList;

        protected Ast<Token> Body => Node[2];

        protected Expression Name => Constant(new Symbol(NameNode.Value.Value));

        protected Expression Nesting => Compiler.BuildNesting();

        protected ClassCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Compile()
	    {
            var scope = new ClassScope(Compiler);
            var classVar = scope.Module as ParameterExpression;
            var header = Assign(classVar, GetClass());

            Compiler.StartScope(scope);

            try
            {
                var body = Body.Accept(Compiler);
                return scope.CompileBody(Block(header, body));
            }
            finally
            {
                Compiler.EndScope();
            }
	    }

        protected Expression CompileSuperclass()
        {
            if(!HasSuperclass)
            {
                return Constant(null, typeof(Class));
            }

            var superclassExpr = SuperclassNode.Accept(Compiler);
            return Expressions.SuperclassCast(superclassExpr);
        }

        protected abstract Expression GetClass();

        private static Class SuperclassCast(iObject super)
        {
            if(super is Class)
            {
                return super as Class;
            }

            throw new TypeError($"superclass must be a Class ({super.Class} given)");
        }

        public static class Reflection
        {
            public static readonly MethodInfo SuperclassCast = Reflector.Method(
                () => SuperclassCast(default(iObject))
            );
        }

        public static class Expressions
        {
            public static MethodCallExpression SuperclassCast(Expression super) =>
                Call(Reflection.SuperclassCast, super);
        }
    }
}
