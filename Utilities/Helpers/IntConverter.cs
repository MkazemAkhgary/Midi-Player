namespace Utilities.Helpers
{
    using Properties;

    public static class IntConverter
    {
        public static byte Reverse(byte input)
        {
            return input;
        }

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

        public static long Reverse(long input)
        {
            var uinput = (ulong)input;
            return (long)(((uinput & 0xFF) << 56) |
                       ((uinput & 0xFF00) << 40) |
                     ((uinput & 0xFF0000) << 24) |
                   ((uinput & 0xFF000000) << 8) |
                 ((uinput & 0xFF00000000) >> 8) |
               ((uinput & 0xFF0000000000) >> 24) |
             ((uinput & 0xFF000000000000) >> 40) |
           ((uinput & 0xFF00000000000000) >> 56));
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
