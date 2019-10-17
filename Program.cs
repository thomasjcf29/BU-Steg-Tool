using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

class Program
{
    private Image coverImage;
    private AnswerFile file;

    private Boolean encrypting;

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

        if(!coverImage.isValid()){
            System.Environment.Exit(99);
        }

        file = new AnswerFile(args[2], encrypting);

        if(!file.isValid()){
            System.Environment.Exit(98);
        }
        file.close();
    }

    private Boolean checkValidation(string[] args)
    {
        //Check to make sure all arguments are provided
        if(args.Length != 3)
        {
            displayArguments();
            return false;
        }

        //Check to make sure encryption / decryption type is provided.
        if(args[0].Equals("Encryption", StringComparison.InvariantCultureIgnoreCase)){
            encrypting = true;
        }
        else if(args[0].Equals("Decryption", StringComparison.InvariantCultureIgnoreCase)){
            encrypting = false;
        }
        else{
            displayArguments();
            return false;
        }

        return true;
    }

    private void displayArguments()
    {
        Console.Error.WriteLine("[ERROR]: Please provide arguments to this program.");
        Console.Error.WriteLine("The correct arguments are:");
        Console.Error.WriteLine("FrankStore.exe <encryption/decryption> <coverImage> <enc/dec file>");
    }
}