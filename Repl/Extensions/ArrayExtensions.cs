namespace Mint.Extensions
{
    static class ArrayExtensions
    {
        public static T[] Subarray<T>(this T[] ary, int offset, int length)
        {
            var new_ary = new T[length];
            System.Array.Copy(ary, offset, new_ary, 0, length);
            return new_ary;
        }
    }
}
