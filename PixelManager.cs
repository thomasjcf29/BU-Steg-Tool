using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Security.Cryptography;
using System.IO;

public class PixelManager
{
    private FrankEncoding encoder;

    private int imageWidth;
    private int imageHeight;
    private Boolean valid = false;

    Dictionary<String, HexCharacter> characterBreakdown = new Dictionary<String, HexCharacter>();
    Dictionary<String, PixelInformation> pixelMap = new Dictionary<String, PixelInformation>();

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

        //Choose Initial Pixels
        choosePixels(10);

        //Setup Manager For Each Letter
        setupHexCharacters();
    }

    public Boolean isValid()
    {
        return valid;
    }

    public FrankEncoding getParent()
    {
        return encoder;
    }

    public List<PixelInformation> getPixels()
    {
        return pixelMap.Values.ToList();
    }

    private void choosePixels(int amount)
    {
        using (RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider())
        {
            for (int i = 0; i < amount; i++)
            {
                byte[] data = new byte[100];
                Boolean invalid = true;

                do
                {
                    //X
                    provider.GetBytes(data);
                    ulong value = BitConverter.ToUInt64(data, 0);
                    int x = (int)(value % Convert.ToUInt64(imageWidth));

                    //Y
                    provider.GetBytes(data);
                    value = BitConverter.ToUInt64(data, 0);
                    int y = (int)(value % Convert.ToUInt64(imageHeight));

                    string key = x.ToString() + "-" + y.ToString();

                    try
                    {
                        PixelInformation px = new PixelInformation(this, x, y);
                        pixelMap.Add(key, px);
                    }
                    //If it's duplicated we need a different one
                    catch(ArgumentException)
                    {
                        continue;
                    }

                    invalid = false;

                } while (invalid);
            }
        }
    }

    private void setupHexCharacters()
    {
        for(int i = 0; i < 16; i++)
        {
            string hex = Converter.intToHex(i);
            HexCharacter hexC = new HexCharacter(this, hex);
            characterBreakdown.Add(hex, hexC);
        }
    }
}