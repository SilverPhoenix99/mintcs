using System;
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

        protected abstract Expression Container { get; }

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
                return scope.CompileBody(Expression.Block(header, body));
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
                Expression.Call(Reflection.SuperclassCast, super);
        }
    }

    internal class SimpleNameClassCompiler : ClassCompiler
    {
        protected override Expression Container => Compiler.CurrentScope.Module;

        public SimpleNameClassCompiler(Compiler compiler) : base(compiler)
        { }

        protected override Expression GetClass() =>
            Module.Expressions.GetOrCreateClass(Container, Name, CompileSuperclass(), Nesting);
    }

    internal class AbsoluteNameClassCompiler : SimpleNameClassCompiler
    {
        protected override Ast<Token> NameNode => Node[0][0];

        protected override Expression Container => Constant(Class.OBJECT);

        public AbsoluteNameClassCompiler(Compiler compiler) : base(compiler)
        { }
    }

    internal class RelativeNameClassCompiler : ClassCompiler
    {
        protected Ast<Token> LeftNode => Node[0][0];

        protected override Ast<Token> NameNode => Node[0][1];

        protected override Expression Container => LeftNode.Accept(Compiler);

        public RelativeNameClassCompiler(Compiler compiler) : base(compiler)
        { }

        protected override Expression GetClass() =>
            Module.Expressions.GetOrCreateClassWithParentCast(Container, Name, CompileSuperclass(), Nesting);
    }

    internal class SingletonClassCompiler : ClassCompiler
    {
        protected override Expression Container { get { throw new NotImplementedException(); } }

        private Ast<Token> OperandNode => Node[0][0];

        public SingletonClassCompiler(Compiler compiler) : base(compiler)
        { }

        protected override Expression GetClass() => Object.Expressions.SingletonClass(OperandNode.Accept(Compiler));
    }
}
