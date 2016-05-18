namespace Mint.Reflection
{
    public class Arity
    {
        public int Minimum { get; }
        public int Maximum { get; }

        public Arity(int minimum, int maximum)
        {
            Minimum = minimum;
            Maximum = maximum;
        }

        public override string ToString() => Maximum == int.MaxValue ? $"{Minimum}+" : $"{Minimum}..{Maximum}";

        public bool Include(int number) => Minimum <= number && number <= Maximum;

        public override bool Equals(object obj) => Equals(obj as Arity);

        public bool Equals(Arity other) => other?.Minimum == Minimum && other?.Maximum == Maximum;

        public override int GetHashCode()
        {
            return Minimum.GetHashCode() << 9 ^ Maximum.GetHashCode();
        }
    }
}
