using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

public class FrankEncoding : SteganographyManager
{
    private PixelManager manager;

    public FrankEncoding(String coverFile, String inputFile, String outputFile)
        : base(coverFile, SteganographyManager.ACTION.ENCODING, inputFile, outputFile)
    {
        if(isValid())
        {
            Console.WriteLine("Initialising encoder.");

            manager = new PixelManager(this);

            if(!manager.isValid())
            {
                setValid(false);
            }
        }
    }

    public void encode()
    {
        //String encodedMessage = Converter.asciiToHex("hello");

        //Console.WriteLine("Choosing which pixels to use, this could take a very long time!");
        //List<Location> locations = manager.encode(encodedMessage);
        //Console.WriteLine(locations.ToString());
    }
}