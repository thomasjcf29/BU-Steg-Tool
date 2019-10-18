using System;
using System.Collections.Generic;
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

    public int getCount()
    {
        return pixels.Count();
    }
}