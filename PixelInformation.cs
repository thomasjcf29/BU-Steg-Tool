using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Text;

namespace FrankStore
{
    /// <summary>
    /// <c>PixelInformation</c> is the class which manages the individual pixels and their attributes.
    /// There is one class to each pixel to be used.
    /// </summary>
    public class PixelInformation
    {
        /// <summary>
        /// Reference to the parent (PixelManager).
        /// </summary>
        private readonly PixelManager parent;

        /// <summary>
        /// X Location of this pixel in the parent image.
        /// </summary>
        private readonly int x;

        /// <summary>
        /// Y Location of this pixel in the parent image.
        /// </summary>
        private readonly int y;

        /// <summary>
        /// Breakdown of the count of hex letters 0-F.
        /// </summary>
        private readonly int[] count = new int[16];

        /// <summary>
        /// Breakdown of all locations of each hex letter 0-F which hasn't been used so far.
        /// </summary>
        private readonly Dictionary<string, List<int>> letterLocations = new Dictionary<string, List<int>>();

        /// <summary>
        /// Hex color of this pixel from the parent image.
        /// </summary>
        private string hexColor;

        /// <summary>
        /// Hash of the pixel string (width-height-color).
        /// </summary>
        private string hash;

        /// <summary>
        /// Breakdown of the hash into a char array.
        /// </summary>
        private readonly char[] breakDown;

        /// <summary>
        /// <c>PixelInformation</c> initiliser takes the specified pixel and generates its unique information.
        /// </summary>
        /// <param name="par">Parent object (PixelManager)</param>
        /// <param name="x">X Location of the pixel</param>
        /// <param name="y">Y Location of the pixel</param>
        /// <returns>Initilised <c>PixelInformation</c> class.</returns>
        public PixelInformation(PixelManager par, int x, int y)
        {
            parent = par;

            this.x = x;
            this.y = y;

            getImageInformation();
            setupLetterMap();
            breakDown = hash.ToCharArray();
        }

        /// <summary>
        /// Returns the count of each hex letter 0-F which hasn't been used yet.
        /// </summary>
        /// <returns>Array of the count of letters 0-F.</returns>
        public int[] getLetterCount()
        {
            return count;
        }

        /// <summary>
        /// Returns a location for a provided hex letter. The method will work out where next the letter comes in the hash and provide it as a location.
        /// </summary>
        /// <param name="letter">Letter to be returned as a location.</param>
        /// <returns><c>Location</c> of the letter ready to be written to file.</returns>
        public Location getLetterLocation(string letter)
        {
            var hexNumber = Converter.hexToInt(letter);

            var hashLocation = letterLocations[letter][0];

            if ((count[hexNumber] <= 0) || (hashLocation == -1))
            {
                Console.Error.WriteLine("[ERROR]: System miscalculated, do not trust this encoding.");
                Environment.Exit(94);
            }

            letterLocations[letter].RemoveAt(0);
            count[hexNumber]--;

            return new Location(x, y, hashLocation);
        }

        /// <summary>
        /// Returns a hex letter from a provided hash location.
        /// </summary>
        /// <param name="number">Hash location to return letter from.</param>
        /// <returns>A string of the hex letter at the provided hash location.</returns>
        public string getLetter(int number)
        {
            return breakDown[number].ToString();
        }

        /// <summary>
        /// This method maps out all hex letters 0-F to all possible locations in the hash.
        /// This makes it easier later on to quickly get a location for a specified hex letter.
        /// </summary>
        private void setupLetterMap()
        {
            for (var i = 0; i < 16; i++)
            {
                var hex = Converter.intToHex(i);

                //For The Actual Class Management (Speed of Service)
                letterLocations.Add(hex, new List<int>());

                var location = 0;
                var letterCount = 0;

                foreach (var c in hash)
                {
                    if (c.ToString().Equals(hex))
                    {
                        letterLocations[hex].Add(location);
                        letterCount++;
                    }

                    location++;
                }

                //For The Parent System
                count[i] = letterCount;
            }
        }

        /// <summary>
        /// This method generates the hash of the specified pixel.
        /// </summary>
        private void getImageInformation()
        {
            var image = parent.getParent().getImage();

            var color = image.getPixel(x, y);
            hexColor = Image.getColorHex(color);

            var source = new StringBuilder();
            source.Append(x.ToString());
            source.Append(y.ToString());
            source.Append(hexColor);

            using (var sha512Hash = SHA512.Create())
            {
                var sourceBytes = Encoding.UTF8.GetBytes(source.ToString());
                var hashBytes = sha512Hash.ComputeHash(sourceBytes);
                hash = BitConverter.ToString(hashBytes).Replace("-", string.Empty);
            }
        }
    }
}