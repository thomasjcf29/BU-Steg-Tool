using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

public class Program
{
    private Image coverImage;
    private AnswerFile file;

    private FrankEncoding encoder;
    private FrankDecoding decoder;

    private Boolean encoding;

    static void Main(string[] args)
    {
        new Program(args);
    }

    public Program(string[] args)
    {
        if(!checkValidation(args))
        {
            System.Environment.Exit(100);
        }

        coverImage = new Image(args[1]);

        if(!coverImage.isValid())
        {
            System.Environment.Exit(99);
        }

        file = new AnswerFile(args[2], encoding);

        if(!file.isValid())
        {
            System.Environment.Exit(98);
        }

        if(encoding)
        {
            encoder = new FrankEncoding(this, args[3]);
        }
        
        else
        {
            decoder = new FrankDecoding(this, args[3]);
        }

        file.close();
    }
    
    public AnswerFile getAnswerFile()
    {
        return file;
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
        if(args[0].Equals("Encoding", StringComparison.InvariantCultureIgnoreCase))
        {
            encoding = true;
        }
        else if(args[0].Equals("Decoding", StringComparison.InvariantCultureIgnoreCase))
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
        Console.Error.WriteLine("FrankStore.exe <encoding/decoding> <coverImage> <enc/dec file> <message to encode>");
    }
}