using System;
using System.Collections.Generic;

namespace FrankStore
{
    public class Program
    {
        private bool encoding;

        // ReSharper disable once InconsistentNaming - Main method has to be capitals
        public static void Main(string[] args)
        {
            // ReSharper disable once ObjectCreationAsStatement - Don't need to keep this assignment once it's done the program can close.
            new Program(args);
        }

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

        private static void displayArguments()
        {
            Console.Error.WriteLine("[ERROR]: Please provide arguments to this program.");
            Console.Error.WriteLine("The correct arguments are:");
            Console.Error.WriteLine("To encode: FrankStore.exe encode <coverImage> <fileToEncode> <outputFile>");
            Console.Error.WriteLine("To decode: FrankStore.exe decode <coverImage> <fileToDecode> <outputFile>");
        }
    }
}