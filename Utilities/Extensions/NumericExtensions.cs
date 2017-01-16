using JetBrains.Annotations;

namespace Utilities.Extensions
{
    public static class NumericExtensions
    {
        public static short Reverse(short input)
        {
            var uinput = (ushort)input;
            return (short)(((uinput & 0xFF) << 8) |
                         ((uinput & 0xFF00) >> 8));
        }

        public static int Reverse(int input)
        {
            var uinput = (uint)input;
            return (int)(((uinput & 0xFF) << 24) |
                       ((uinput & 0xFF00) << 8) |
                     ((uinput & 0xFF0000) >> 8) |
                   ((uinput & 0xFF000000) >> 24));
        }

        public static int ByteArrayToInt([NotNull]byte[] input)
        {
            var val = 0;

            for (int i = 0; i < input.Length; i++)
            {
                val <<= 8;
                val |= input[i];
            }

            return val;
        }
    }
}
