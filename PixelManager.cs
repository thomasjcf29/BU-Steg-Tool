using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

public class PixelManager
{
    private FrankEncoding encoder;

    private int imageWidth;
    private int imageHeight;
    private Boolean valid = false;

    private List<PixelInformation> a;

    public PixelManager(FrankEncoding enc)
    {
        encoder = enc;

        Image image = encoder.getParent().getImage();

        imageWidth = image.getWidth();
        imageHeight = image.getHeight();

        if((imageWidth * imageHeight) < 10)
        {
            Console.Error.WriteLine("[ERROR]: The image does not have more than 10 pixels.");
            return;
        }

        valid = true;
    }

    public Boolean isValid()
    {
        return valid;
    }

    public FrankEncoding getParent()
    {
        return encoder;
    }
}