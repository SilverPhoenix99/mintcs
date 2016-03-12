namespace Mint.Compiler
{
    class Condition
    {
        public bool Valid { get; private set; } = true;

        public void Invalidate()
        {
            Valid = false;
        }
    }
}
