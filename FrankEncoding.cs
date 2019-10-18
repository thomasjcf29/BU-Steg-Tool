using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

public class FrankEncoding
{
    private Program parent;

    public FrankEncoding(Program parent, String message)
    {
        this.parent = parent;

        new PixelInformation(this, 500, 400);
    }

    public Program getParent()
    {
        return parent;
    }
}