namespace Mint.Parser
{
    public struct BitStack
    {
        private ulong stack;

        public bool Peek => (stack & 1) == 1;

        public void Push(bool value)
        {
            stack = (stack << 1) | (value ? 1ul : 0);
        }

        public bool Pop()
        {
            var peek = Peek;
            stack >>= 1;
            return peek;
        }

        public bool LexPop()
        {
            var peek = Peek;
            stack = (stack >> 1) | (stack & 1);
            return peek;
        }
    }
}
