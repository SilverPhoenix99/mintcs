using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Mint.Parser
{
    public interface AstVisitor<T>
    {
        void Visit(Ast<T> node);
    }

    public class Ast<T> : IEnumerable<Ast<T>>
    {
        public Ast() { }

        public Ast(T value)
        {
            Value = value;
        }

        public Ast(T value, IEnumerable<Ast<T>> children) : this(value)
        {
            ((List<Ast<T>>) List).AddRange(children);
        }

        public T Value { get; }
        public IReadOnlyList<Ast<T>> List { get; } = new List<Ast<T>>();
        public Ast<T> this[int index] => List[index];

        public void Accept(AstVisitor<T> visitor)
        {
            visitor.Visit(this);
        }

        // Adds the element to the end of the list.
        public Ast<T> Append(Ast<T> other) =>
            Value == null && other.Value == null
            ? new Ast<T>(Value, List.Concat(other.List))
            : new Ast<T>(Value, List.Concat(new[] { other }));
        
        // Append operator
        public static Ast<T> operator +(Ast<T> left, Ast<T> right) => left?.Append(right) ?? right;
        
        public static explicit operator Ast<T>(T value) => new Ast<T>(value);

        public IEnumerator<Ast<T>> GetEnumerator() => List.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override string ToString()
        {
            var s = Value == null ? "" : Value.ToString();
            s += " -> (";
            if(List.Count != 0)
            {
                s += List.Select(ast => ast.ToString()).Aggregate((current, next) => current + ", " + next);
            }
            s += ")";

            return s;
        }
    }
}
