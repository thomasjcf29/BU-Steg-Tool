using System;

namespace FrankStore
{
    public class FrankEncoding : SteganographyManager
    {
        private readonly PixelManager manager;

        public FrankEncoding(string coverFile, string inputFile, string outputFile)
            : base(coverFile, Action.Encoding, inputFile, outputFile)
        {
            if (!isValid()) return;
            
            Console.WriteLine("Initialising encoder.");
            manager = new PixelManager(this);

            if (manager.isValid()) return;
            
            setValid(false);
        }

        public void encode()
        {
            var startTime = DateTime.Now;

            Console.WriteLine("Reading the file and generating the pixels, this may take a while!");

            while (!(getInputFile().isFileRead()))
            {
                var hex = Converter.byteToHex(getInputFile().getBytes());

                var locations = manager.encode(hex);
                getOutputFile().writeToFile(locations);
            }

            var totalTime = (DateTime.Now - startTime).TotalMilliseconds;

            Console.WriteLine("\nThis has been completed in " + totalTime.ToString("F1") + " milliseconds");
        }
    }
}