using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

public class Image
{
    private Bitmap image;
    private Boolean valid = false;

    public Image(String location) {
        checkValid(location);
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

    public Boolean isValid()
    {
        return valid;
    }

    private Boolean checkValid(String location){
        if (!File.Exists(location))
        {
            Console.Error.WriteLine("[ERROR]: File does not exist for specified image");
            return false;
        }

        try
        {
            image = new Bitmap(location);
        }
        catch (ArgumentException)
        {
            Console.Error.WriteLine("[ERROR]: Specified image file is not valid, is it corrupted?");
            return false;
        }

        valid = true;
        return true;
    }
}