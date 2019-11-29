using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace FrankStore
{
    /// <summary>
    /// <c>PixelManager</c> is incharge of managing all the pixels during the encoding/decoding process.
    /// </summary>
    public class PixelManager
    {
        /// <summary>
        /// Reference to the parent <c>SteganographyManager</c>.
        /// </summary>
        private readonly SteganographyManager parent;

        /// <summary>
        /// Cover image total width.
        /// </summary>
        private int imageWidth;

        /// <summary>
        /// Cover image total height.
        /// </summary>
        private int imageHeight;

        /// <summary>
        /// Boolean status of whether this class was successfully setup, if false do not use.
        /// </summary>
        private readonly bool valid;

        /// <summary>
        /// Contains a breakdown of all hex letters (0-F) and their <c>HexCharacter</c> manager.
        /// </summary>
        private readonly Dictionary<string, HexCharacter> characterBreakdown = new Dictionary<string, HexCharacter>();

        /// <summary>
        /// Contains a map of pixel x,y locations to <c>PixelInformation</c>.
        /// Useful for when decoding.
        /// </summary>
        private readonly Dictionary<string, PixelInformation> pixelMap = new Dictionary<string, PixelInformation>();

        /// <summary>
        /// <c>PixelManager</c> initiliser, creates the class and returns it.
        /// </summary>
        /// <para>
        /// It should be noted that if the action was created under encoding then it will generate 1000 random pixels to use.
        /// </para>
        /// <param name="manager">The parent manager <c>SteganographyManager</c> or any children.</param>
        public PixelManager(SteganographyManager manager)
        {
            Console.WriteLine("Initialising pixel manager.");

            parent = manager;

            //Setup Class Params
            setupClass();

            if (parent.getAction() == SteganographyManager.Action.Encoding)
            {
                //Setup Manager For Each Letter
                setupHexCharacters();

                //Choose Initial Pixels
                Console.WriteLine("Choosing 1000 random pixels (this may increase later on).");
                addPixels(1000);
                Console.WriteLine("");
            }

            valid = true;
        }

        /// <summary>
        /// Returns whether this class was successfully setup or not, if false do not use!
        /// </summary>
        /// <returns>true for successfully setup, for false for not.</returns>
        public bool isValid()
        {
            return valid;
        }

        /// <summary>
        /// Gets the parent object who created this class.
        /// </summary>
        /// <returns><c>SteganographyManager</c> who created this class.</returns>
        public SteganographyManager getParent()
        {
            return parent;
        }

        /// <summary>
        /// Returns back all the locations for a specified hex message.
        /// </summary>
        /// <param name="message">The hex characters to be encoded.</param>
        /// <returns>A list of locations to write to file.</returns>
        public IEnumerable<Location> encode(string message)
        {
            return message.Select(c => characterBreakdown[c.ToString()].chooseHexCharacter()).ToList();
        }

        /// <summary>
        /// Converts a list of locations back to their hex characters.
        /// </summary>
        /// <param name="locations">List of locations read from file.</param>
        /// <returns>A string of hex characters.</returns>
        public string decode(IEnumerable<Location> locations)
        {
            var sb = new StringBuilder();

            foreach (var loc in locations)
            {

                var x = Convert.ToInt32(loc.getX());
                var y = Convert.ToInt32(loc.getY());
                var hashLocation = Convert.ToInt32(loc.getHashLocation());

                if (x >= imageWidth || y >= imageHeight || hashLocation >= 128)
                {
                    Console.Error.WriteLine("[ERROR]: Decode file has invalid sizes.");
                    Environment.Exit(91);
                }

                var key = x + "-" + y;

                PixelInformation px;

                try
                {
                    px = pixelMap[key];
                }
                catch (KeyNotFoundException)
                {
                    px = new PixelInformation(this, x, y);
                    pixelMap.Add(key, px);
                }

                sb.Append(px.getLetter(hashLocation));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Adds more pixels from the original image file.
        /// </summary>
        /// <param name="amount">Amount of pixels to add from parent image.</param>
        public void addPixels(int amount)
        {
            if (pixelMap.Count + 1 + amount > getImageSize())
            {
                Console.Error.WriteLine(
                    "[ERROR]: This image is not large enough to hide the data, please start again with a bigger image.");
                Environment.Exit(97);
            }

            using (var provider = new RNGCryptoServiceProvider())
            {
                for (var i = 0; i < amount; i++)
                {
                    var data = new byte[100];
                    var invalid = true;

                    do
                    {
                        //X
                        provider.GetBytes(data);
                        var value = BitConverter.ToUInt64(data, 0);
                        var x = (int) (value % Convert.ToUInt64(imageWidth));

                        //Y
                        provider.GetBytes(data);
                        value = BitConverter.ToUInt64(data, 0);
                        var y = (int) (value % Convert.ToUInt64(imageHeight));

                        var key = x + "-" + y;

                        try
                        {
                            var px = new PixelInformation(this, x, y);
                            pixelMap.Add(key, px);
                        }
                        //If it's duplicated we need a different one
                        catch (ArgumentException)
                        {
                            continue;
                        }

                        invalid = false;

                    } while (invalid);
                }

                updateHexCharacters();
            }
        }

        /// <summary>
        /// Generates the image size (width * height)
        /// </summary>
        /// <returns>Returns the image size (width * height)</returns>
        private int getImageSize()
        {
            return imageWidth * imageHeight;
        }

        /// <summary>
        /// Updates the <c>HexCharacter</c> with all pixels which can be used for that letter.
        /// There are multiple pixel entries if a pixel can be used more than once.
        /// </summary>
        private void updateHexCharacters()
        {
            var overall = new List<PixelInformation>[16];

            for (var i = 0; i < 16; i++)
            {
                overall[i] = new List<PixelInformation>();
            }

            foreach (var pi in pixelMap.Values.ToList())
            {
                var count = pi.getLetterCount();

                for (var i = 0; i < 16; i++)
                {
                    for (var y = 0; y < count[i]; y++)
                    {
                        overall[i].Add(pi);
                    }
                }
            }

            for (var i = 0; i < 16; i++)
            {
                characterBreakdown[Converter.intToHex(i)].updatePixels(overall[i]);
            }
        }

        /// <summary>
        /// Generic setup of the class to get parent image information and calculate basic information.
        /// </summary>
        private void setupClass()
        {
            Image image = parent.getImage();

            imageWidth = image.getWidth();
            imageHeight = image.getHeight();

            if (imageWidth * imageHeight >= 10) return;
            
            Console.Error.WriteLine("[ERROR]: The image does not have more than 10 pixels.");
        }

        /// <summary>
        /// Creates the basic hollow layout of the hex characters.
        /// </summary>
        private void setupHexCharacters()
        {
            for (var i = 0; i < 16; i++)
            {
                var hex = Converter.intToHex(i);
                var hexC = new HexCharacter(this, hex);
                characterBreakdown.Add(hex, hexC);
            }
        }
    }
}