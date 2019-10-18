using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

public class PixelInformation
{
	private FrankEncoding parentEncoder;
    private FrankDecoding parentDecoder;

    private int x;
    private int y;

    private string hexColor;
    private string hash;

    public PixelInformation(FrankEncoding enc, UInt16 ux, UInt16 uy)
    {
        parentEncoder = enc;
        parentDecoder = null;

        x = Convert.ToInt32(ux);
        y = Convert.ToInt32(uy);

        Image image = parentEncoder.getParent().getImage();

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

        Console.WriteLine(hash);
    }

    public PixelInformation(FrankDecoding dec, UInt16 ux, UInt16 uy)
    {
        parentEncoder = null;
        parentDecoder = dec;

        x = Convert.ToInt32(ux);
        y = Convert.ToInt32(uy);

        Image image = parentDecoder.getParent().getImage();

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