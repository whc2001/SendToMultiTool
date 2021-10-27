using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendToMultiTool
{
    internal static class Utils
    {
        public static T[] SubArray<T>(this T[] array, int offset, int length)
        {
            T[] result = new T[length];
            Array.Copy(array, offset, result, 0, length);
            return result;
        }

        public static UInt32 ReverseBytes(UInt32 value)
        {
            return (value & 0x000000FFU) << 24 | (value & 0x0000FF00U) << 8 |
                   (value & 0x00FF0000U) >> 8 | (value & 0xFF000000U) >> 24;
        }

        public static byte[] ReverseBytes(byte[] value)
        {
            byte[] copy = new byte[value.Length];
            value.CopyTo(copy, 0);
            Array.Reverse(copy);
            return copy;
        }

        public static string ByteArrayToHexString(byte[] data)
        {
            return BitConverter.ToString(data).Replace("-", "");
        }

        public static bool IsDirectory(string fullPath)
        {
            FileAttributes attr = File.GetAttributes(fullPath);
            return (attr & FileAttributes.Directory) == FileAttributes.Directory;
        }

        public static byte XorHash(byte[] payload)
        {
            byte ret = 0x00;
            for (int i = 0; i < payload.Length; ++i)
                ret ^= payload[i];
            return ret;
        }
    }
}
