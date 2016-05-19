namespace Mint.MethodBinding
{
    public class Arguments
    {
        public Array Splat { get; }
        public Hash KeySplat { get; }
        public iObject Block { get; }

        public Arguments(Array splat, Hash keySplat, iObject block)
        {
            Splat = splat;
            KeySplat = keySplat;
            Block = block;
        }
    }
}