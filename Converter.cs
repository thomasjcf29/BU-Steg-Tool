using System;
using System.Text;

namespace FrankStore
{
    public class Converter
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

        public static string hexToAscii(string input)
        {
            var sb = new StringBuilder();

            for (var i = 0; i < input.Length; i += 2)
            {
                var charConvert = input.Substring(i, 2);

                var lNumber = Convert.ToInt32(charConvert, 16);
                var c = (char) lNumber;

                sb.Append(c.ToString());
            }

            return sb.ToString();
        }
    }
}