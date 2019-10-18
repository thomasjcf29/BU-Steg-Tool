using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

public class FrankEncoding
{
    private Program parent;
    private PixelManager manager;

    private Boolean valid = false;

    public FrankEncoding(Program parent, String message)
    {
        this.parent = parent;
        manager = new PixelManager(this);

        if(manager.isValid())
        {
            valid = true;
        }
    }

    public Program getParent()
    {
        return parent;
    }

    public Boolean isValid()
    {
        return valid;
    }
}