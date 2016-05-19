namespace Mint.MethodBinding
{
    public class Arguments
    {
        public iObject[] Splat { get; }
        public LinkedDictionary<iObject, iObject> KeySplat { get; }
        public iObject Block { get; }

        public Arguments(iObject[] splat, LinkedDictionary<iObject, iObject> keySplat, iObject block)
        {
            Splat = splat;
            KeySplat = keySplat;
            Block = block;
        }
    }
}