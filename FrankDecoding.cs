using System;

namespace FrankStore
{
    /// <summary>
    /// FrankDecoding Class is the decoding manager to ensure that the an encoded file is provided back to its original state.
    /// </summary>
    /// <para>
    /// Provided of course that the end user has the same cover image that was used to encode the file.
    /// </para>
    public class FrankDecoding : SteganographyManager
    {
        /// <summary>
        /// Reference to the child pixel manager, this is the manager which will hold / manage all the pixel information.
        /// </summary>
        private readonly PixelManager manager;

        /// <summary>
        /// The constructor for the <c>FrankDecoding</c> class, this will initialise the class with all the required values.
        /// </summary>
        /// <param name="coverFile">The file used to decode the information (the image key).</param>
        /// <param name="inputFile">The file you want to decode.</param>
        /// <param name="outputFile">The file you would like to output it to.</param>
        /// <returns>An initialised <c>FrankDecoding</c> object.</returns>
        public FrankDecoding(string coverFile, string inputFile, string outputFile)
            : base(coverFile, Action.Decoding, inputFile, outputFile)
        {
            if (!isValid()) return;
            
            Console.WriteLine("Initialising decoder.");

            manager = new PixelManager(this);

            if(manager.isValid()) return;
            
            setValid(false);
        }

        /// <summary>
        /// The decode method will begin reading the encoded file, parsing it and then outputting it to the decoded file.
        /// </summary>
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