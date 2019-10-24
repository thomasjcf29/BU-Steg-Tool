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
        DateTime startTime = DateTime.Now;

        Console.WriteLine("Reading the file and generating the pixels, this may take a while!");

        while(!(getInputFile().isFileRead()))
        {
            String hex = Converter.byteToHex(getInputFile().getBytes());

            List<Location> locations = manager.encode(hex);
            getOutputFile().writeToFile(locations);
        }

        close();

        double totalTime = (DateTime.Now - startTime).TotalMilliseconds;

        Console.WriteLine("\nThis has been completed in " + totalTime.ToString("F1") + " milliseconds");
    }
}