using System;
using System.Collections.Generic;
using System.Text;

namespace FrankStore
{
    /// <summary>
    /// Class <c>Converter</c> is in charge of converting from specified inputs to outputs.
    /// </summary>
    /// <para>
    /// For example, this class can convert byte to hex and back as well as hex to int.
    /// </para>
    public static class Converter
    {
        /// <summary>
        /// Returns back the hex representation of the provided bytes.
        /// </summary>
        /// <param name="input">Bytes you wish to convert to their hex representation.</param>
        /// <returns>Hex representation of the provided bytes.</returns>
        public static string byteToHex(IEnumerable<byte> input)
        {
            var sb = new StringBuilder();

            foreach (var b in input)
            {
                sb.Append($"{b:X2}");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Returns back the byte[] representation of the provided hex.
        /// </summary>
        /// <param name="input">Hex you wish to convert to their bytes representation.</param>
        /// <returns>Byte representation of the hex you provided.</returns>
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

        /// <summary>
        /// Returns back the hex representation of the int you provided.
        /// </summary>
        /// <para>
        /// I.E. 0 = 0, 16 = F.
        /// </para>
        /// <param name="input">Int you would like to convert to hex.</param>
        /// <returns>Hex representation of the int you provided.</returns>
        public static string intToHex(int input)
        {
            return input.ToString("X");
        }

        /// <summary>
        /// Returns back the int representation of the hex you provided.
        /// </summary>
        /// <para>
        /// I.E. F = 16, 0 = 0.
        /// </para>
        /// <param name="input">Hex you would like to convert to an int.</param>
        /// <returns>Int representation of the hex you provided.</returns>
        public static int hexToInt(string input)
        {
            return int.Parse(input, System.Globalization.NumberStyles.HexNumber);
        }
    }
}