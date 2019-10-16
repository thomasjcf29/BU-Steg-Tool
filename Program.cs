using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace FrankStore
{
    class Program
    {
        static void Main(string[] args)
        {
            Bitmap img = new Bitmap(args[0]);

            for(int i = 0; i < img.Height; i++){
                for(int y = 0; y < img.Width; y++){
                    Console.WriteLine(y + ", " + i);
                    Color color = img.GetPixel(y, i);
                    String result = System.Drawing.ColorTranslator.ToHtml(color);
                    Console.WriteLine(result);
                    Console.WriteLine();
                }
            }
         }
    }
}
