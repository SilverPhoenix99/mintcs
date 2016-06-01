using System.Linq.Expressions;
using static System.Linq.Expressions.Expression;

namespace Mint.Compilation
{
    public enum ScopeType
    {
        Method = 1,
        While,
        For,
        Block,
        Module
    }

    public class Scope
    {
        private LabelTarget breakLabel;
        private LabelTarget nextLabel;
        private LabelTarget redoLabel;
        private LabelTarget retryLabel;

        public ScopeType Type { get; }

        public Scope Previous { get; private set; }

        public Closure Closure { get; }

        public LabelTarget BreakLabel
        {
            get { return breakLabel ?? (BreakLabel = Label(typeof(iObject), "break")); }
            set { breakLabel = value; }
        }

        public LabelTarget NextLabel
        {
            get { return nextLabel ?? (NextLabel = Label("next")); }
            set { nextLabel = value; }
        }

        public LabelTarget RedoLabel
        {
            get { return redoLabel ?? (RedoLabel = Label("redo")); }
            set { redoLabel = value; }
        }

        public LabelTarget RetryLabel
        {
            get { return retryLabel ?? (RetryLabel = Label("retry")); }
            set { retryLabel = value; }
        }

        public Scope(ScopeType type, Closure closure)
        {
            Type = type;
            Closure = closure;
        }

        public Scope Enter(ScopeType type, Closure closure = null) =>
            new Scope(type, closure ?? new Closure(Closure.Self)) { Previous = this };
    }
}
