using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation.Components
{
    internal class SafeMethodCallCompiler : PublicMethodCallCompiler
    {
        public SafeMethodCallCompiler(Compiler compiler) : base(compiler)
        { }

        public override Expression Reduce()
        {
            var left = GetLeftExpression();
            var arguments = PopArguments();
            var methodName = new Symbol(MethodName);
            var visibility = GetVisibility();

            var instance = Variable(typeof(iObject), "instance");
            var checkNilInstance = Call(CompilerUtils.IS_NIL, instance.Cast<object>());
            var call = CompilerUtils.Call(instance, methodName, visibility, arguments);
            var conditionalCall = Condition(checkNilInstance, Compiler.NIL, call, typeof(iObject));

            return Block(
                typeof(iObject),
                new[] { instance },
                Assign(instance, left),
                conditionalCall
            );
        }
    }
}
