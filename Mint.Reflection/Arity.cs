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

        public bool Include(int number) => Minimum <= number && number <= Maximum;
        
        public Arity Merge(Arity other)
        {
            var min = Minimum;
            if(other.Minimum < min) min = other.Minimum;

            var max = Maximum;
            if(other.Maximum > max) max = other.Maximum;

            return new Arity(min, max);
        }

        public override bool Equals(object obj) => Equals(obj as Arity);

        public bool Equals(Arity other) => other?.Minimum == Minimum && other?.Maximum == Maximum;

        public override int GetHashCode()
        {
            return Minimum.GetHashCode() << 9 ^ Maximum.GetHashCode();
        }
        
        public override string ToString() => Maximum == int.MaxValue ? $"{Minimum}+" : $"{Minimum}..{Maximum}";
    }
}
