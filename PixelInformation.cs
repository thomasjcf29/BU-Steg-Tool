using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Text;

namespace FrankStore
{
    public class PixelInformation
    {
        private readonly PixelManager parent;

        private readonly int x;
        private readonly int y;

        private readonly int[] count = new int[16];

        private readonly Dictionary<string, List<int>> letterLocations = new Dictionary<string, List<int>>();

        private string hexColor;
        private string hash;
        private readonly char[] breakDown;

        public PixelInformation(PixelManager par, int x, int y)
        {
            parent = par;

            this.x = x;
            this.y = y;

            getImageInformation();
            setupLetterMap();
            breakDown = hash.ToCharArray();
        }

        public int[] getLetterCount()
        {
            return count;
        }

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

        public string getLetter(int number)
        {
            return breakDown[number].ToString();
        }

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