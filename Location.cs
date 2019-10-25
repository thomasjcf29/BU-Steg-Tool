using System;
using System.Text;

namespace FrankStore
{
    public class Location
    {
        private ushort xLocation;
        private ushort yLocation;
        private ushort hashLocation;

        public Location(ushort x, ushort y, ushort hashLoc)
        {
            xLocation = x;
            yLocation = y;
            hashLocation = hashLoc;
        }

        public Location(int x, int y, int hashLoc)
        {
            xLocation = Convert.ToUInt16(x);
            yLocation = Convert.ToUInt16(y);
            hashLocation = Convert.ToUInt16(hashLoc);
        }

        public ushort getX()
        {
            return xLocation;
        }

        public ushort getY()
        {
            return yLocation;
        }

        public ushort getHashLocation()
        {
            return hashLocation;
        }

        public void setX(ushort x)
        {
            xLocation = x;
        }

        public void setY(ushort y)
        {
            yLocation = y;
        }

        public void setHashLocation(ushort hash)
        {
            hashLocation = hash;
        }

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