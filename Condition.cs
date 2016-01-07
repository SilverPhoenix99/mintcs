namespace mint
{
    class Condition
    {
        public bool Valid { get; private set; }

        public Condition()
        {
            Valid = true;
        }

        public void Invalidate()
        {
            Valid = false;
        }
    }
}
