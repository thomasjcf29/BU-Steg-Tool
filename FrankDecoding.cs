using System;

namespace FrankStore
{
    public class FrankDecoding : SteganographyManager
    {
        private readonly PixelManager manager;

        public FrankDecoding(string coverFile, string inputFile, string outputFile)
            : base(coverFile, Action.Decoding, inputFile, outputFile)
        {
            if (!isValid()) return;
            
            Console.WriteLine("Initialising decoder.");

            manager = new PixelManager(this);

            if(manager.isValid()) return;
            
            setValid(false);
        }

        public void decode()
        {
            var startTime = DateTime.Now;

            Console.WriteLine("Decoding and writing file, this may take a while!");

            while (!(getInputFile().isFileRead()))
            {
                var locations = getInputFile().getLocations();
                var bytes = Converter.hexToByte(manager.decode(locations));
                getOutputFile().writeToFile(bytes);
            }

            var totalTime = (DateTime.Now - startTime).TotalMilliseconds;

            Console.WriteLine("\nThis has been completed in " + totalTime.ToString("F1") + " milliseconds");
        }
    }
}