using System;
using System.Text;

namespace FrankStore
{
    public static class Converter
    {
        public static string byteToHex(byte[] input)
        {
            var sb = new StringBuilder();

            foreach (var b in input)
            {
                sb.Append($"{b:X2}");
            }

            return sb.ToString();
        }

        public static byte[] hexToByte(string input)
        {
            var charNumber = input.Length;
            var bytes = new byte[charNumber / 2];
            for (var i = 0; i < charNumber; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(input.Substring(i, 2), 16);
            }

            return bytes;
        }

        public static string intToHex(int input)
        {
            return input.ToString("X");
        }

        public static int hexToInt(string input)
        {
            return int.Parse(input, System.Globalization.NumberStyles.HexNumber);
        }
    }
}