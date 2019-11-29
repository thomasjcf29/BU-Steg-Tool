using System;

namespace FrankStore
{
    /// <summary>
    /// <c>FrankEncoding</c> class is the encoding manager which will take the file you want to encode and then output it
    /// encoded using the image you provided.
    /// </summary>
    public class FrankEncoding : SteganographyManager
    {
        /// <summary>
        /// The <c>PixelManager</c> is in charge of collecting and managing all the pixels to work out where to encode
        /// the information.
        /// </summary>
        private readonly PixelManager manager;

        /// <summary>
        /// The constructor for <c>FrankEncoding</c> will collect the 3 provided file locations, open / create them
        /// and then initialise the <c>PixelManager</c>.
        /// </summary>
        /// <param name="coverFile">The file you want to encode using.</param>
        /// <param name="inputFile">The file you wish to encode.</param>
        /// <param name="outputFile">The outputted encoded file.</param>
        /// <returns>An initialised <c>FrankEncoding</c> file.</returns>
        public FrankEncoding(string coverFile, string inputFile, string outputFile)
            : base(coverFile, Action.Encoding, inputFile, outputFile)
        {
            if (!isValid()) return;
            
            Console.WriteLine("Initialising encoder.");
            manager = new PixelManager(this);

            if (manager.isValid()) return;
            
            setValid(false);
        }

        /// <summary>
        /// The encode method will process the decoded input file, encode it using the image and then output it to a file.
        /// </summary>
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