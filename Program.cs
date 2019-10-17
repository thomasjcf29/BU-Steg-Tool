using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

class Program
{
    Image coverImage;

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

        coverImage = new Image(args[0]);

        if(!coverImage.isValid()){
            System.Environment.Exit(99);
        }
    }

    private Boolean checkValidation(string[] args)
    {
        if(args.Length == 0)
        {
            Console.Error.WriteLine("[ERROR]: Please provide arguments to this program.");
            return false;
        }
        return true;
    }
}