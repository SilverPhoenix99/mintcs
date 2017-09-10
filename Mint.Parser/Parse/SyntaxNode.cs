using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Mint.Parse
{
    public class SyntaxNode : IEnumerable<SyntaxNode>
    {
        public SyntaxNode(Token token, IEnumerable<SyntaxNode> children = null)
        {
            Token = token;

            if(children != null)
            {
                Append(children);
            }
        }

        public SyntaxNode(Token token, params SyntaxNode[] children)
            : this(token, (IEnumerable<SyntaxNode>) children)
        { }

        public SyntaxNode(params SyntaxNode[] children)
            : this(null, (IEnumerable<SyntaxNode>) children)
        { }

        public Token Token { get; }
        public IReadOnlyList<SyntaxNode> List { get; } = new List<SyntaxNode>();
        public SyntaxNode this[int index] => List[index];
        public bool IsList => Token == null;
        
        public TRet Accept<TRet>(AstVisitor<TRet> visitor) => visitor.Visit(this);
        
        public void Accept(AstVisitor visitor) => visitor.Visit(this);

        // Adds the element to the end of the list.
        public SyntaxNode Append(SyntaxNode other)
        {
            if(IsList && other.IsList)
            {
                return Append(other.List);
            }
            
            ((List<SyntaxNode>) List).Add(other);
            return this;
        }

        public SyntaxNode Append(params SyntaxNode[] children) => Append((IEnumerable<SyntaxNode>) children);

        public SyntaxNode Append(IEnumerable<SyntaxNode> children)
        {
            ((List<SyntaxNode>) List).AddRange(children);
            return this;
        }

        // Append operator
        public static SyntaxNode operator +(SyntaxNode left, SyntaxNode right) => left?.Append(right) ?? right;

        //public static explicit operator SyntaxNode(Token value) => new SyntaxNode(value);

        public IEnumerator<SyntaxNode> GetEnumerator() => List.GetEnumerator();
        
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override string ToString()
        {
            var s = Token?.ToString() ?? "";
            s += " : [";
            if(List.Count != 0)
            {
                s += " ";
                s += List.Select(ast => ast.ToString()).Aggregate((current, next) => current + ", " + next);
                s += " ";
            }
            s += "]";

            return s;
        }
    }
}
