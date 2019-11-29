using System;
using System.Collections.Generic;

namespace FrankStore
{
    /// <summary>
    /// Class <c>Program</c> create the object to manage the program.
    /// </summary>
    /// <para>
    /// This class is what will file off the Encoding/Decoding class and hand all permissions down to those classes.
    /// </para>
    public class Program
    {
        private bool encoding;

        /// <summary>
        /// Static entry method to this program, will instantly create the <c>Program</c> class.
        /// </summary>
        /// <param name="args">
        /// Arguments to be passed into the program.
        /// </param>
        // ReSharper disable once InconsistentNaming - Main method has to be capitals
        public static void Main(string[] args)
        {
            // ReSharper disable once ObjectCreationAsStatement - Don't need to keep this assignment once it's done the
            // program can close.
            new Program(args);
        }

        /// <summary>
        /// Class initiator this method creates a new <c>Program</c> class.
        /// </summary>
        /// <param name="args">
        /// Arguments to be passed into the program.
        /// </param>
        /// <returns>
        /// The class <c>Program</c>.
        /// </returns>
        private Program(IReadOnlyList<string> args)
        {
            Console.WriteLine("Welcome To FrankStore!");

            if (!checkValidation(args))
            {
                Environment.Exit(100);
            }

            runTasks(args[1], args[2], args[3]);

            Console.WriteLine("Operation completed, closing.");
        }

        /// <summary>
        /// Run tasks will take the parameters and either encode or decode the file.
        /// </summary>
        /// <para>
        /// If encoding it will create the <c>FrankEncoding</c> class, if decoding it will create the <c>FrankDecoding</c>
        /// class.
        /// </para>
        /// <param name="coverFile">The image to use which hides the data.</param>
        /// <param name="inputFile">The file you wish to encode/decode.</param>
        /// <param name="outputFile">The file you wish to output it too.</param>
        private void runTasks(string coverFile, string inputFile, string outputFile)
        {
            if (encoding)
            {
                Console.WriteLine("You have chosen to encode a file.");

                var encoder = new FrankEncoding(coverFile, inputFile, outputFile);

                if (!encoder.isValid()) return;
                
                encoder.encode();
                encoder.close();
            }
            else
            {
                Console.WriteLine("You have chosen to decode a file.");

                var decoder = new FrankDecoding(coverFile, inputFile, outputFile);

                if (!decoder.isValid()) return;
                
                decoder.decode();
                decoder.close();
            }
        }

        /// <summary>
        /// Checks the input arguments to ensure all the required values are provided.
        /// </summary>
        /// <param name="args">The arguments provided to the program</param>
        /// <returns>Returns <c>true</c> if the arguments are valid or <c>false</c> if they are not.</returns>
        private bool checkValidation(IReadOnlyList<string> args)
        {
            //Check to make sure all arguments are provided
            if (args.Count != 4)
            {
                displayArguments();
                return false;
            }

            //Check to make sure encoding / decoding type is provided.
            if (args[0].Equals("encode", StringComparison.InvariantCultureIgnoreCase))
            {
                encoding = true;
            }
            else if (args[0].Equals("decode", StringComparison.InvariantCultureIgnoreCase))
            {
                encoding = false;
            }
            else
            {
                displayArguments();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Displays a message to console saying what the valid arguments are with examples.
        /// </summary>
        private static void displayArguments()
        {
            Console.Error.WriteLine("[ERROR]: Please provide arguments to this program.");
            Console.Error.WriteLine("The correct arguments are:");
            Console.Error.WriteLine("To encode: FrankStore.exe encode <coverImage> <fileToEncode> <outputFile>");
            Console.Error.WriteLine("To decode: FrankStore.exe decode <coverImage> <fileToDecode> <outputFile>");
        }
    }
}