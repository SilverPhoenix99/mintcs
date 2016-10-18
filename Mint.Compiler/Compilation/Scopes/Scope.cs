using System.Collections.Generic;
using System.Linq.Expressions;

namespace Mint.Compilation.Scopes
{
    public interface Scope
    {
        Scope Parent { get; }

        ParameterExpression Nesting { get; }

        LabelTarget BreakLabel { get; }

        //LabelTarget NextLabel { get; }

        //LabelTarget RedoLabel { get; }

        //LabelTarget RetryLabel { get; }
    }

    public class TopLevelScope : Scope
    {
        public Scope Parent => this;

        public ParameterExpression Nesting { get; }

        public LabelTarget BreakLabel { get { throw new SyntaxError("Invalid break"); } }

        public LabelTarget NextLabel { get { throw new SyntaxError("Invalid next"); } }

        public LabelTarget RedoLabel { get { throw new SyntaxError("Invalid redo"); } }

        public LabelTarget RetryLabel { get { throw new SyntaxError("Invalid retry"); } }

        public TopLevelScope()
        {
            Nesting = Expression.Variable(typeof(IList<Module>), "nesting");
        }
    }

    public class MethodScope : Scope
    {
        public Scope Parent { get; }

        public ParameterExpression Nesting => Parent.Nesting;

        public LabelTarget BreakLabel { get { throw new SyntaxError("Invalid break"); } }

        public LabelTarget NextLabel { get { throw new SyntaxError("Invalid next"); } }

        public LabelTarget RedoLabel { get { throw new SyntaxError("Invalid redo"); } }

        public LabelTarget RetryLabel { get { throw new SyntaxError("Invalid retry"); } }

        public MethodScope(Scope parent)
        {
            Parent = parent;
        }
    }

    public class BlockScope : MethodScope
    {
        public BlockScope(Scope parent) : base(parent)
        { }
    }

    public class WhileScope : MethodScope
    {
        public WhileScope(Scope parent) : base(parent)
        { }
    }

    public class ForScope : WhileScope
    {
        public ForScope(Scope parent) : base(parent)
        { }
    }

    public class ModuleScope : Scope
    {
        public Scope Parent { get; }

        public ParameterExpression Nesting { get; }

        public LabelTarget BreakLabel { get { throw new System.NotImplementedException(); } }

        public ModuleScope(Scope parent)
        {
            Parent = parent;
            Nesting = Expression.Variable(typeof(IList<Module>), "nesting");
        }
    }

    public class ClassScope : ModuleScope
    {
        public ClassScope(Scope parent) : base(parent)
        { }
    }
}
