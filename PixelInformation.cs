using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

public class PixelInformation
{
	private PixelManager parent;

    private int x;
    private int y;

    private string hexColor;
    private string hash;

    public PixelInformation(PixelManager par, UInt16 ux, UInt16 uy)
    {
        parent = par;

        x = Convert.ToInt32(ux);
        y = Convert.ToInt32(uy);

        Image image = parent.getParent().getParent().getImage();

        Color color = image.getPixel(x, y);
        hexColor = image.getColorHex(color);

        StringBuilder source = new StringBuilder();
        source.Append(x.ToString());
        source.Append(y.ToString());
        source.Append(hexColor);

        using (SHA512 sha512Hash = SHA512.Create())
        {
            byte[] sourceBytes = Encoding.UTF8.GetBytes(source.ToString());
            byte[] hashBytes = sha512Hash.ComputeHash(sourceBytes);
            hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty);
        }
    }
}