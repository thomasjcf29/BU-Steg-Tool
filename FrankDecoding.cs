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
        DateTime startTime = DateTime.Now;

        Console.WriteLine("Decoding and writing file, this may take a while!");

        while(!(getInputFile().isFileRead()))
        {
            DateTime startBlock = DateTime.Now;
            List<Location> locations = getInputFile().getLocations();
            String hex = manager.decode(locations);
            double totalBlockTime = (DateTime.Now - startBlock).TotalSeconds;
            Console.WriteLine("\nThis has been completed in " + totalBlockTime.ToString("F1") + " seconds");
            //Console.WriteLine(hex);
        }

        close();

        double totalTime = (DateTime.Now - startTime).TotalMilliseconds;

        Console.WriteLine("\nThis has been completed in " + totalTime.ToString("F1") + " milliseconds");

        //Console.WriteLine("");
        //String hex = manager.decode(locations);
        //Console.WriteLine("Hex has been retreived.");

        //Console.WriteLine("Converting back to ASCII.");
        //String ascii = Converter.hexToAscii(hex);
        //Console.WriteLine("Done.");
    }
}