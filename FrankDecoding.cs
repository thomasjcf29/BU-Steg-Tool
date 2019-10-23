using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

public class FrankDecoding : SteganographyManager
{
    private PixelManager manager;

    public FrankDecoding(String coverFile, String inputFile, String outputFile)
        : base(coverFile, SteganographyManager.ACTION.DECODING, inputFile, outputFile)
    {
        if(isValid())
        {
            Console.WriteLine("Initialising decoder.");

            manager = new PixelManager(this);

            if(!manager.isValid())
            {
                setValid(false);
            }
        }
    }

    public void decode()
    {
        //Console.WriteLine("Decoding file, this may take a while!");
        //String hex = manager.decode(locations);
        //Console.WriteLine("Hex has been retreived.");

        //Console.WriteLine("Converting back to ASCII.");
        //String ascii = Converter.hexToAscii(hex);
        //Console.WriteLine("Done.");
    }
}