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

    public Program(string[] args){
        coverImage = new Image(args[0]);
        Console.WriteLine(coverImage.getPixel(500, 40));
        Color color = coverImage.getPixel(500, 40);
        Console.WriteLine(coverImage.getColorHex(color));
        Console.WriteLine(coverImage.getWidth());
        Console.WriteLine(coverImage.getHeight());
    }
}