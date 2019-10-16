using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

public class Image
{
    private Bitmap image;

    public Image(String location) {
        image = new Bitmap(location);
    }

    public Color getPixel(int width, int height)
    {
        return image.GetPixel(width, height);
    }

    public String getColorHex(Color pixelColor)
    {
        return System.Drawing.ColorTranslator.ToHtml(pixelColor);
    }

    public int getHeight()
    {
        return image.Height;
    }

    public int getWidth()
    {
        return image.Width;
    }
}