using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

public class HexCharacter
{
    private PixelManager parent;

    private List<PixelInformation> pixels = new List<PixelInformation>();

    private string letter;

    public HexCharacter(PixelManager par, string let)
    {
        parent = par;
        letter = let;

        updatePixels();
    }

    public void updatePixels()
    {
        pixels.Clear();

        foreach (PixelInformation pixel in parent.getPixels())
        {
            int count = pixel.getLetterCount(letter);

            for(int i = 0; i < count; i++)
            {
                pixels.Add(pixel);
            }
        }
    }

    public Location chooseHexCharacter()
    {
        int length = pixels.Count;

        if(length == 0)
        {
            Console.Error.WriteLine("[ERROR]: There was a problem encoding the information, we have ran out of pixels.");
            System.Environment.Exit(96);
        }

        PixelInformation pixel;

        using (RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider())
        {
            byte[] data = new byte[100];

            //Pixel To use
            provider.GetBytes(data);
            ulong value = BitConverter.ToUInt64(data, 0);
            int x = (int)(value % Convert.ToUInt64(length));

            pixel = pixels[x];
        }

        pixels.Remove(pixel);

        Location loc = pixel.getLetterLocation(letter);

        if(loc == null)
        {
            Console.Error.WriteLine("[ERROR]: There was a problem generating the location information, do not trust this encoding.");
            System.Environment.Exit(95);
        }

        checkPixelCount();

        return loc;
    }

    public int getCount()
    {
        return pixels.Count();
    }

    private void checkPixelCount()
    {
        if(pixels.Count < 10)
        {
            parent.addPixels(10);
        }
    }
}