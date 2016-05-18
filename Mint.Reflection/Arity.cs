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
    }
}
