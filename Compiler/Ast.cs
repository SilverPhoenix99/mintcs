using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace mint.Compiler
{
    public interface AstVisitor<T>
    {
        void Visit(AstNode<T> node);
        void Visit(AstList<T> list);
    }

    public abstract class Ast<T>
    {
        public abstract void Accept(AstVisitor<T> visitor);

        // Adds the element to the end of the list.
        // It it is a list, it will be added as a single node.
        public abstract Ast<T> Append(Ast<T> other);

        // Joins two nodes together. Lists are flattened.
        public abstract Ast<T> Concat(Ast<T> other);

        // Append operator
        public static Ast<T> operator |(Ast<T> left, Ast<T> right) => left?.Append(right) ?? right;

        // Concat operator
        public static Ast<T> operator +(Ast<T> left, Ast<T> right) => left?.Concat(right) ?? right;
    }

    public class AstNode<T> : Ast<T>
    {
        public T Value { get; }

        public AstNode(T value) { Value = value; }

        public override void Accept(AstVisitor<T> visitor) { visitor.Visit(this); }

        public override Ast<T> Append(Ast<T> other) => new AstList<T>(this).Append(other);

        public override Ast<T> Concat(Ast<T> other) => new AstList<T>(this).Concat(other);

        public static implicit operator AstNode<T>(T value) => new AstNode<T>(value);
    }

    public class AstList<T> : Ast<T>, IEnumerable<Ast<T>>
    {
        public IReadOnlyList<Ast<T>> List { get; }

        public Ast<T> this[int index] => List[index];

        public AstList(IEnumerable<Ast<T>> list) { List = new List<Ast<T>>(list); }

        public AstList(params Ast<T>[] list) : this((IEnumerable<Ast<T>>) list) { }

        public override void Accept(AstVisitor<T> visitor) { visitor.Visit(this); }

        public override Ast<T> Append(Ast<T> other) => other == null
                                                    ? this
                                                    : new AstList<T>( List.Concat(new[] { other }) );

        public override Ast<T> Concat(Ast<T> other)
        {
            if(other is AstList<T>)
            {
                var otherList = ((AstList<T>) other).List;
                return new AstList<T>( List.Concat(otherList) );
            }

            return Append(other);
        }

        public IEnumerator<Ast<T>> GetEnumerator() => List.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
