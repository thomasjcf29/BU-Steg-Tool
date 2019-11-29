using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace FrankStore
{
    /// <summary>
    /// <c>HexCharacter</c> is used to collect all the pixel locations for a certain hex character.
    /// </summary>
    /// <para>
    /// This is important for example, if provided the hex letter "A". It needs to quickly find a location to use. This
    /// class will find all the valid locations that can be used to store that information.
    /// </para>
    public class HexCharacter
    {
        /// <summary>
        /// The parent <c>PixelManager</c> this is important to be able to get important objects like the Cover Image
        /// (<c>Image</c>)
        /// </summary>
        private readonly PixelManager parent;

        /// <summary>
        /// Holds a list of all pixels which can still be used to hide information for this letter.
        /// </summary>
        /// <para>
        /// This is NOT a complete list of all pixels used so far, only the pixels which can still be used. This list
        /// will also have the same amount of the same pixel object for how many times it can store the letter. For example,
        /// if this specific instance was in charge of hex 'F', and one pixel could store F 16 times. That pixel would
        /// be stored 16 times in this list.
        /// </para>
        private List<PixelInformation> pixels = new List<PixelInformation>();

        /// <summary>
        /// The hex letter / number this class is in charge of hiding. For example ("F").
        /// </summary>
        private readonly string letter;

        /// <summary>
        /// The constructor for the <c>HexCharacter</c> class which will return back a HexCharacter object.
        /// </summary>
        /// <param name="par">The PixelManager which initialised it.</param>
        /// <param name="let">The letter this object is in charge of.</param>
        /// <returns>
        /// An initialised <c>HexCharacter</c> class.
        /// </returns>
        public HexCharacter(PixelManager par, string let)
        {
            parent = par;
            letter = let;
        }

        /// <summary>
        /// Updates the pixel list of all available pixels which can be used. Normally gets called when the amount of available pixels has been increased.
        /// </summary>
        /// <param name="px">List of pixels which can be used.</param>
        public void updatePixels(List<PixelInformation> px)
        {
            pixels.Clear();
            pixels = px;
        }

        /// <summary>
        /// Returns a location of the next hex character this class can provide.
        /// </summary>
        /// <returns>Location of next available hex character.</returns>
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

        /// <summary>
        /// Checks how many pixels are left for this character - if less than 10 - requests <c>PixelManager</c> to add some more.
        /// </summary>
        private void checkPixelCount()
        {
            if (pixels.Count < 10)
            {
                parent.addPixels(1000);
            }
        }
    }
}