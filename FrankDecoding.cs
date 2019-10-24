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
            List<Location> locations = getInputFile().getLocations();
            byte[] bytes = Converter.hexToByte(manager.decode(locations));
            getOutputFile().writeToFile(bytes);
        }

        double totalTime = (DateTime.Now - startTime).TotalMilliseconds;

        Console.WriteLine("\nThis has been completed in " + totalTime.ToString("F1") + " milliseconds");
    }
}