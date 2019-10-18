using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

public class Converter
{   
    public static string asciiToHex(string input)
    {
        StringBuilder sb = new StringBuilder();

        byte[] inputBytes = Encoding.UTF8.GetBytes(input);

        foreach (byte b in inputBytes)
        {
            sb.Append(string.Format("{0:X2}", b));
        }

        return sb.ToString();
    }

    public static string intToHex(int input)
    {
        return input.ToString("X");
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