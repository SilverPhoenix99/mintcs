namespace Mint.Reflection
{
    public class Arity
    {
        public Arity(int minimum, int maximum)
        {
            Minimum = minimum;
            Maximum = maximum;
        }


        public int Minimum { get; }
        public int Maximum { get; }


        public bool Include(int number)
            => Minimum <= number && number <= Maximum;
        

        public Arity Merge(Arity other)
        {
            var min = Minimum;
            if(other.Minimum < min) min = other.Minimum;

            var max = Maximum;
            if(other.Maximum > max) max = other.Maximum;

            return new Arity(min, max);
        }


        public override bool Equals(object obj) => Equals(obj as Arity);


        public bool Equals(Arity other) => other?.Minimum == Minimum && other.Maximum == Maximum;


        public override int GetHashCode()
        {
            var hash = 23;
            hash = hash * 31 + Minimum.GetHashCode();
            return hash * 31 + Maximum.GetHashCode();
        }
        

        public override string ToString()
        {
            return Minimum == Maximum ? Minimum.ToString()
                 : Maximum == int.MaxValue ? $"{Minimum}+"
                 : $"{Minimum}..{Maximum}";
        }
    }
}
