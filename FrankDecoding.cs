using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

public class FrankDecoding
{
    private Program parent;
    private PixelManager manager;

    private Boolean valid = false;

    public FrankDecoding(Program parent)
    {
        Console.WriteLine("Initialising decoder.");

        this.parent = parent;
        manager = new PixelManager(this);

        if(manager.isValid())
        {
            valid = true;
        }
    }

    public String decoder(List<Location> locations)
    {
        Console.WriteLine("Decoding file, this may take a while!");
        string hex = manager.decode(locations);
        Console.WriteLine("Hex has been retreived.");

        Console.WriteLine("Converting back to ASCII.");
        string ascii = Converter.hexToAscii(hex);
        Console.WriteLine("Done.");
        
        return ascii;
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