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

    public FrankEncoding(Program parent)
    {
        Console.WriteLine("Initialising encoder.");

        this.parent = parent;
        manager = new PixelManager(this);

        if(manager.isValid())
        {
            valid = true;
        }
    }

    public List<Location> encode(String message)
    {
        String encodedMessage = Converter.asciiToHex(message);
        
        Console.WriteLine("Choosing which pixels to use, this could take a very long time!");
        List<Location> locations = manager.encode(encodedMessage);
        Console.WriteLine("");

        return locations;
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