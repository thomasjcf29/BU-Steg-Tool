using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

public class Program
{
    private Boolean encoding;

    static void Main(string[] args)
    {
        new Program(args);
    }

    public Program(string[] args)
    {
        Console.WriteLine("Welcome To FrankStore!");

        if(!checkValidation(args))
        {
            System.Environment.Exit(100);
        }

        runTasks(args[1], args[2], args[3]);

        Console.WriteLine("Operation completed, closing.");
    }

    private void runTasks(String coverFile, String inputFile, String outputFile)
    {
        if(encoding)
        {
            Console.WriteLine("You have chosen to encode a file.");

            FrankEncoding encoder = new FrankEncoding(coverFile, inputFile, outputFile);

            if(encoder.isValid())
            {
                encoder.encode();
                encoder.close();
            }
        }
        else
        {
            Console.WriteLine("You have chosen to decode a file.");

            FrankDecoding decoder = new FrankDecoding(coverFile, inputFile, outputFile);

            if(decoder.isValid())
            {
                decoder.decode();
                decoder.close();
            }
        }
    }

    private Boolean checkValidation(string[] args)
    {
        //Check to make sure all arguments are provided
        if(args.Length != 4)
        {
            displayArguments();
            return false;
        }

        //Check to make sure encoding / decoding type is provided.
        if(args[0].Equals("encode", StringComparison.InvariantCultureIgnoreCase))
        {
            encoding = true;
        }
        else if(args[0].Equals("decode", StringComparison.InvariantCultureIgnoreCase))
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

    private void displayArguments()
    {
        Console.Error.WriteLine("[ERROR]: Please provide arguments to this program.");
        Console.Error.WriteLine("The correct arguments are:");
        Console.Error.WriteLine("To encode: FrankStore.exe encode <coverImage> <fileToEncode> <outputFile>");
        Console.Error.WriteLine("To decode: FrankStore.exe decode <coverImage> <fileToDecode> <outputFile>");
    }
}