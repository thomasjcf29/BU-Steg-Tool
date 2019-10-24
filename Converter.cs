using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

public class Converter
{   
    public static string byteToHex(byte[] input)
    {
        StringBuilder sb = new StringBuilder();

        foreach (byte b in input)
        {
            sb.Append(string.Format("{0:X2}", b));
        }

        return sb.ToString();
    }

    public static byte[] hexToByte(String input)
    {
        int charNumber = input.Length;
        byte[] bytes = new byte[charNumber / 2];
        for(int i = 0; i < charNumber; i += 2)
        {
            bytes[i/2] = Convert.ToByte(input.Substring(i, 2), 16);
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
        StringBuilder sb = new StringBuilder();

        for(int i = 0; i < input.Length; i = i + 2)
        {
            string charConvert = input.Substring(i, 2);

            int lNumber = Convert.ToInt32(charConvert, 16);
            char c = (char) lNumber;

            sb.Append(c.ToString());
        }

        return sb.ToString();
    }
}