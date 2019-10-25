using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;

namespace FrankStore
{
    public class HexCharacter
    {
        private readonly PixelManager parent;

        private List<PixelInformation> pixels = new List<PixelInformation>();

        private readonly string letter;

        public HexCharacter(PixelManager par, string let)
        {
            parent = par;
            letter = let;
        }

        public void updatePixels(List<PixelInformation> px)
        {
            pixels.Clear();
            pixels = px;
        }

        public Location chooseHexCharacter()
        {
            var length = pixels.Count;

            if (length == 0)
            {
                Console.Error.WriteLine(
                    "[ERROR]: There was a problem encoding the information, we have ran out of pixels.");
                Environment.Exit(96);
            }

            PixelInformation pixel;

            using (var provider = new RNGCryptoServiceProvider())
            {
                var data = new byte[100];

                //Pixel To use
                provider.GetBytes(data);
                var value = BitConverter.ToUInt64(data, 0);
                var x = (int) (value % Convert.ToUInt64(length));

                pixel = pixels[x];
                pixels.RemoveAt(x);
            }

            var loc = pixel.getLetterLocation(letter);

            if (loc == null)
            {
                Console.Error.WriteLine(
                    "[ERROR]: There was a problem generating the location information, do not trust this encoding.");
                Environment.Exit(95);
            }

            checkPixelCount();

            return loc;
        }

        public int getCount()
        {
            return pixels.Count();
        }

        private void checkPixelCount()
        {
            if (pixels.Count < 10)
            {
                parent.addPixels(1000);
            }
        }
    }
}