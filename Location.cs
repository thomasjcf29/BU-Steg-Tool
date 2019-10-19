using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Location
{
    private UInt16 xLocation;
    private UInt16 yLocation;
    private UInt16 hashLocation;

    public Location(UInt16 x, UInt16 y, UInt16 hashLoc)
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

    public UInt16 getX()
    {
        return xLocation;
    }

    public UInt16 getY()
    {
        return yLocation;
    }

    public UInt16 getHashLocation()
    {
        return hashLocation;
    }

    public void setX(UInt16 x)
    {
        xLocation = x;
    }

    public void setY(UInt16 y)
    {
        yLocation = y;
    }

    public void setHashLocation(UInt16 hash)
    {
        hashLocation = hash;
    }

    public string toString()
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