namespace Utilities.Extensions
{
    public static class NumericExtensions
    {
        // this overload is here to prevent using other overloads when byte is passed as parameter.
        public static byte Reverse(byte input)
        {
            return input;
        }

        public static short Reverse(short input)
        {
            var uinput = (ushort)input;
            return (short)(((uinput & 0x00FF) << 8) |
                           ((uinput & 0xFF00) >> 8));
        }

        public static int Reverse(int input)
        {
            var uinput = (uint)input;
            return (int)(((uinput & 0x000000FF) << (32 - 08 - 0)) |
                         ((uinput & 0x0000FF00) << (32 - 16 - 8)) |
                         ((uinput & 0x00FF0000) >> (32 - 16 - 8)) |
                         ((uinput & 0xFF000000) >> (32 - 08 - 0)));
        }

        public static long Reverse(long input)
        {
            var uinput = (ulong)input;
            return (long) (((uinput & 0x00000000000000FF) << (64 - 08 - 00)) |
                           ((uinput & 0x000000000000FF00) << (64 - 16 - 08)) |
                           ((uinput & 0x0000000000FF0000) << (64 - 24 - 16)) |
                           ((uinput & 0x00000000FF000000) << (64 - 32 - 24)) |

                           ((uinput & 0x000000FF00000000) >> (64 - 32 - 24)) |
                           ((uinput & 0x0000FF0000000000) >> (64 - 24 - 16)) |
                           ((uinput & 0x00FF000000000000) >> (64 - 16 - 08)) |
                           ((uinput & 0xFF00000000000000) >> (64 - 08 - 00)));
        }
    }
}
