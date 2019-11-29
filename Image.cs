using System;
using System.Drawing;
using System.IO;

namespace FrankStore
{
    /// <summary>
    /// <c>Image</c> is used to open the image used for encoding / decoding.
    /// </summary>
    public class Image
    {
        /// <summary>
        /// Holds the reference to the image file which is being read.
        /// </summary>
        private Bitmap image;

        /// <summary>
        /// Boolean to hold whether the class initialisation was valid.
        /// </summary>
        private bool valid;

        /// <summary>
        /// The constructor for <c>Image</c> which returns back a initilised class.
        /// </summary>
        /// <param name="location">File location of the image to be opened.</param>
        /// <returns>
        /// An initilised <c>Image</c> class.
        /// </returns>
        public Image(string location)
        {
            Console.WriteLine("Trying to open specified image cover.");

            checkValid(location);
        }

        /// <summary>
        /// Gets the pixel color of the specified width and height.
        /// </summary>
        /// <param name="width">X Location of the pixel.</param>
        /// <param name="height">Y location of the pixel.</param>
        /// <returns><c>Color</c> class of the chosen pixel.</returns>
        public Color getPixel(int width, int height)
        {
            return image.GetPixel(width, height);
        }

        /// <summary>
        /// Converts <c>Color</c> class to a hex string letter.
        /// </summary>
        /// <param name="pixelColor">Color which should be converted.</param>
        /// <returns>Hex representation (as a string) of the provided color.</returns>
        public static string getColorHex(Color pixelColor)
        {
            return ColorTranslator.ToHtml(pixelColor);
        }

        /// <summary>
        /// Returns height of the image.
        /// </summary>
        /// <returns>Height of the image.</returns>
        public int getHeight()
        {
            return image.Height;
        }

        /// <summary>
        /// Returns width of the image.
        /// </summary>
        /// <returns>Width of the image.</returns>
        public int getWidth()
        {
            return image.Width;
        }

        /// <summary>
        /// Returns whether or not the class could be initilised properly.
        /// </summary>
        /// <para>
        /// If returned false, do not use this class and try to reinitilise.
        /// </para>
        /// <returns>True if initilised or false if it failed.</returns>
        public bool isValid()
        {
            return valid;
        }

        /// <summary>
        /// Basic validation of the location parameter to ensure it is valid.
        /// </summary>
        /// <param name="location">Location of the image file.</param>
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