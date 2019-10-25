using System;
using System.Drawing;
using System.IO;

namespace FrankStore
{
    public class Image
    {
        private Bitmap image;

        private bool valid;

        public Image(string location)
        {
            Console.WriteLine("Trying to open specified image cover.");

            checkValid(location);
        }

        public Color getPixel(int width, int height)
        {
            return image.GetPixel(width, height);
        }

        public string getColorHex(Color pixelColor)
        {
            return ColorTranslator.ToHtml(pixelColor);
        }

        public int getHeight()
        {
            return image.Height;
        }

        public int getWidth()
        {
            return image.Width;
        }

        public bool isValid()
        {
            return valid;
        }

        private void checkValid(string location)
        {
            if (!File.Exists(location))
            {
                Console.Error.WriteLine("[ERROR]: File does not exist for specified image");
                return;
            }

            try
            {
                image = new Bitmap(location);
            }
            catch (ArgumentException)
            {
                Console.Error.WriteLine("[ERROR]: Specified image file is not valid, is it corrupted?");
                return;
            }

            valid = true;
        }
    }
}