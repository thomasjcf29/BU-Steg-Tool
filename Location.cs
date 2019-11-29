using System;
using System.Text;

namespace FrankStore
{
    /// <summary>
    /// <c>Location</c> class is used to easily provide a break down of where the encoding is on the image, ready to be written to the encoding file.
    /// </summary>
    public class Location
    {
        /// <summary>
        /// Location on the x axies (width) of the image, which is being used.
        /// </summary>
        private readonly ushort xLocation;

        /// <summary>
        /// Location on the y axies (height) of the image, which is being used.
        /// </summary>
        private readonly ushort yLocation;

        /// <summary>
        /// Location on the hash of the individual pixel, which is being used.
        /// </summary>
        private readonly ushort hashLocation;

        /// <summary>
        /// Intilises the <c>Location</c> class with the provided x, y and hash locations in there ushort form.
        /// </summary>
        /// <param name="x">Width location of the pixel to use.</param>
        /// <param name="y">Height location of the pixel to use.</param>
        /// <param name="hashLoc">Location on the hash to use.</param>
        /// <returns>
        /// An initilised <c>Location</c> class.
        /// </returns>
        public Location(ushort x, ushort y, ushort hashLoc)
        {
            xLocation = x;
            yLocation = y;
            hashLocation = hashLoc;
        }

        /// <summary>
        /// Intilises the <c>Location</c> class with the provided x, y and hash locations in there int form.
        /// </summary>
        /// <param name="x">Width location of the pixel to use.</param>
        /// <param name="y">Height location of the pixel to use.</param>
        /// <param name="hashLoc">Location on the hash to use.</param>
        /// <returns>
        /// An initilised <c>Location</c> class.
        /// </returns>
        public Location(int x, int y, int hashLoc)
        {
            xLocation = Convert.ToUInt16(x);
            yLocation = Convert.ToUInt16(y);
            hashLocation = Convert.ToUInt16(hashLoc);
        }

        /// <summary>
        /// Get X (width) location of this class.
        /// </summary>
        /// <returns>Width location of this class.</returns>
        public ushort getX()
        {
            return xLocation;
        }

        /// <summary>
        /// Get Y (height) location of this class.
        /// </summary>
        /// <returns>Height location of this class.</returns>
        public ushort getY()
        {
            return yLocation;
        }

        /// <summary>
        /// Get hash location of this class.
        /// </summary>
        /// <returns>Width location of this class.</returns>
        public ushort getHashLocation()
        {
            return hashLocation;
        }

        /// <summary>
        /// Returns a console writable version of this class for debugging.
        /// </summary>
        /// <returns>Console writable version of this class.</returns>
        public override string ToString()
        {
            return new StringBuilder()
                .Append("X: ")
                .Append(xLocation.ToString())
                .Append(", Y: ")
                .Append(yLocation.ToString())
                .Append(", Hash: ")
                .Append(hashLocation.ToString())
                .ToString();
        }
    }
}