using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Security.Cryptography;
using System.IO;

public class PixelManager
{
    private SteganographyManager parent;

    private int imageWidth;
    private int imageHeight;
    private Boolean valid = false;

    Dictionary<String, HexCharacter> characterBreakdown = new Dictionary<String, HexCharacter>();
    Dictionary<String, PixelInformation> pixelMap = new Dictionary<String, PixelInformation>();

    public PixelManager(SteganographyManager managaer)
    {
        Console.WriteLine("Initialising pixel manager.");

        parent = managaer;

        //Setup Class Params
        setupClass(true);

        if(parent.getAction() == SteganographyManager.ACTION.ENCODING)
        {
            //Setup Manager For Each Letter
            setupHexCharacters();

            //Choose Initial Pixels
            Console.WriteLine("Choosing 1000 random pixels (this may increase later on).");
            addPixels(1000);
            Console.WriteLine("");
        }

        valid = true;
    }

    public Boolean isValid()
    {
        return valid;
    }

    public SteganographyManager getParent()
    {
        return parent;
    }

    public List<PixelInformation> getPixels()
    {
        return pixelMap.Values.ToList();
    }

    public List<Location> encode(String message)
    {
        List<Location> locations = new List<Location>();
        
        foreach(char c in message)
        {
            locations.Add(characterBreakdown[c.ToString()].chooseHexCharacter());
        }

        return locations;
    }

    public string decode(List<Location> locations)
    {
        StringBuilder sb = new StringBuilder();

        foreach(Location loc in locations)
        {
            int x = Convert.ToInt32(loc.getX());
            int y = Convert.ToInt32(loc.getY());
            int hashLocation = Convert.ToInt32(loc.getHashLocation());

            if((x >= imageWidth) || (y >= imageHeight) || (hashLocation >= 128))
            {
                Console.WriteLine("-");
                Console.Error.WriteLine("[ERROR]: Decode file has invalid sizes.");
                System.Environment.Exit(91);
            }

            Console.Write("+");

            String key = x.ToString() + "-" + y.ToString();

            PixelInformation px;

            try{
                px = pixelMap[key];
            }
            catch(KeyNotFoundException)
            {
                px = new PixelInformation(this, x, y);
            }

            sb.Append(px.getLetter(hashLocation));
        }

        Console.WriteLine("");

        return sb.ToString();
    }

    public void addPixels(int amount)
    {
        if(((pixelMap.Count() + 1) + amount) > getImageSize())
        {
            Console.Error.WriteLine("[ERROR]: This image is not large enough to hide the data, please start again with a bigger image.");
            System.Environment.Exit(97);
        }

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
                        Console.Write(".");
                        continue;
                    }

                    Console.Write("+");

                    invalid = false;

                } while (invalid);
            }

            updateHexCharacters();
        }
    }

    private int getImageSize()
    {
        return imageWidth * imageHeight;
    }

    private void updateHexCharacters()
    {
        List<PixelInformation>[] overall = new List<PixelInformation>[16];

        for(int i = 0; i < 16; i++)
        {
            overall[i] = new List<PixelInformation>();
        }

        foreach(PixelInformation pi in pixelMap.Values.ToList())
        {
            int[] count = pi.getLetterCount();

            for(int i = 0; i < 16; i ++)
            {
                for(int y = 0; y < count[i]; y++)
                {
                    overall[i].Add(pi);
                }
            }
        }

        for(int i = 0; i < 16; i++)
        {
            characterBreakdown[Converter.intToHex(i)].updatePixels(overall[i]);
        }
    }

    private void setupClass(Boolean encoding)
    {
        Image image = parent.getImage();

        imageWidth = image.getWidth();
        imageHeight = image.getHeight();

        if ((imageWidth * imageHeight) < 10)
        {
            Console.Error.WriteLine("[ERROR]: The image does not have more than 10 pixels.");
            return;
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